using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace messenger
{
    using System.Windows;
    using System.Windows.Controls;

    using TestMessenger;

    class CommuManager
    {
        private enum CommunicationStages
        {
            Error,
            Standby,
            SentENQWaitingEOT,
            SentMsgWaitingAck,
            GotAckWaitingResponseENQ,
            SentEOTWaitingMsg
        }

        private SerialPort MySerialPort { get; set; }

        private string MyPortName { get; set; }

        public int MyBaudRate { get; set; }

        private int MyWriteTimeout { get; set; }

        private int MyReadTimeout { get; set; }

        private StopBits MyStopBits { get; set; }

        private int MyDataBits { get; set; }

        private Parity MyParity { get; set; }

        private CommunicationStages _comState;

        private MainWindow myMainWindow;
        private TextBox myTb;
        private EnqChecker MsgChecker;
        private BlockingCollection<byte[]> msgQueue;

        public CommuManager(string port, MainWindow mainWindow)
        {
            msgQueue = new BlockingCollection<byte[]>();
            myMainWindow = mainWindow;

            myTb = mainWindow.DisplayWindow;
            myTb.Text += "Initializing...\n";
            
            _comState = CommunicationStages.Standby;
            myTb.Text += "Current commu stage: " + _comState + "\n";

            MyPortName = port;
            MyBaudRate = 115200;
            MyWriteTimeout = 500;
            MyReadTimeout = 5000;
            MyStopBits = StopBits.One;
            MyDataBits = 8;
            MyParity = Parity.None;

            MySerialPort = new SerialPort(MyPortName, MyBaudRate, MyParity, MyDataBits, MyStopBits)
            {
                ReadTimeout = MyReadTimeout,
                WriteTimeout = MyWriteTimeout,
                DtrEnable = true,
                RtsEnable = true
            };

            if (!MySerialPort.IsOpen)
            {
                MySerialPort.Open();
            }

            MsgChecker = new EnqChecker(MySerialPort, myMainWindow);
            MsgChecker.MsgrDone += GotMsg;

            Task.Factory.StartNew(() =>
            {
                foreach (byte[] msg in msgQueue.GetConsumingEnumerable())
                {
                    Send(msg);
                }
            });
        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task RunPeriodicAsync(Action onTick,
                                                   TimeSpan dueTime,
                                                   TimeSpan interval,
                                                   CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        private async void Initialize()
        {
            var dueTime = TimeSpan.FromSeconds(2);
            var interval = TimeSpan.FromSeconds(1);

            // TODO: Add a CancellationTokenSource and supply the token here instead of None.
            await RunPeriodicAsync(OnTick, dueTime, interval, CancellationToken.None);
        }

        public void StartAskSensor()
        {
            Initialize();
        }

        public void OnTick()
        {
            var str = GenerateRandomString();

            msgQueue.Add(Encoding.ASCII.GetBytes(str));
        }

        private string GenerateRandomString()
        {
            var result = "";
            var randomNum = new Random();
            for (int i = 0; i < 3; i++)
            {
                result += randomNum.Next(0, 9).ToString();
            }

            return result;
        }

        private void GotMsg()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Got Msg Success!");
            MySerialPort.DataReceived += MsgChecker.Receive;
        }

        private void SentSuccess()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Sent Msg Success!");
            MySerialPort.DataReceived += MsgChecker.Receive;
        }

        public void Send(byte[] msg)
        {
            MySerialPort.DataReceived -= MsgChecker.Receive;
            var sender = new Sender(MySerialPort, msg, myMainWindow);
            sender.MsgrDone += SentSuccess;
        }
    }
}
