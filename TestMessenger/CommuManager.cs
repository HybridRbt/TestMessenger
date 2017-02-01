using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace TestMessenger
{
    using System.Windows.Controls;

    class CommuManager
    {
        public enum CommunicationStages
        {
            Error,
            Standby,
            Busy
        }

        public SerialPort MySerialPort { get; set; }

        private string MyPortName { get; set; }

        public int MyBaudRate { get; set; }

        private int MyWriteTimeout { get; set; }

        private int MyReadTimeout { get; set; }

        private StopBits MyStopBits { get; set; }

        private int MyDataBits { get; set; }

        private Parity MyParity { get; set; }
        public CommunicationStages ComState { get; set; }

        private MainWindow myMainWindow;
        private TextBox myTb;
        private EnqSender MsgChecker;
        private BlockingCollection<byte[]> msgQueue;

        private ComEventHandler comEventHandler;
        private byte[] _nextMsg;
        public CommuManager(string port, MainWindow mainWindow)
        {
            msgQueue = new BlockingCollection<byte[]>();
            myMainWindow = mainWindow;

            myTb = mainWindow.DisplayWindow;
            myTb.Text += "Initializing...\n";
            
            ComState = CommunicationStages.Standby;
            myTb.Text += "Current commu stage: " + ComState + "\n";


            MyPortName = port;
            MyBaudRate = 115200;
            MyWriteTimeout = 50;
            MyReadTimeout = 100;
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

            comEventHandler = new ComEventHandler(this, myMainWindow);
            comEventHandler.GotEnq += SendEot;
            comEventHandler.GotEot += SendMsg;
            comEventHandler.GotMsg += ReceiveMsg;
            comEventHandler.GotAck += ReturnToStandby;

            Task.Factory.StartNew(() =>
            {
                foreach (byte[] msg in msgQueue.GetConsumingEnumerable())
                {
                    Send(msg);
                }
            });
           
            MySerialPort.ErrorReceived += MySerialPort_ErrorReceived;
        }

        private void ReturnToStandby()
        {
            SentSuccess();
            ComState = CommunicationStages.Standby;
        }

        private void SendRequest(byte[] msg)
        {
            _nextMsg = msg;
            var sender = new EnqSender(this, myMainWindow);
            sender.SendEnq();
        }

        public string GenerateStringFromByteArray(byte[] msg)
        {
            var result = "";

            for (int i = 0; i < msg.Length; i++)
            {
                result += msg[i].ToString();
            }

            return result;
        }

        private void ReceiveMsg()
        {
            GotMsg();
            var msgStr = GenerateStringFromByteArray(comEventHandler.MsgReceived);
            var player = new Player(myMainWindow.MsgGot);
            player.Display(msgStr);
            SendAck();
        }

        private void SendAck()
        {
            var sender = new AckSender(this, myMainWindow);
            sender.SendAck();
        }

        private void SendMsg()
        {
            var sender = new MsgSender(this, _nextMsg, myMainWindow);
            sender.SendMsg();
        }

        private void SendEot()
        {
            ComState = CommunicationStages.Busy;
            var sender = new EotSender(this, myMainWindow);
            sender.SendEot();
        }

        private void MySerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
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

        private async void InitializeAskSensorTask()
        {
            var dueTime = TimeSpan.FromSeconds(1);
            var interval = TimeSpan.FromMilliseconds(500);

            // TODO: Add a CancellationTokenSource and supply the token here instead of None.
            try
            {
                await RunPeriodicAsync(OnAskSensorTick, dueTime, interval, cancelAskSensor.Token);
            }
            catch (OperationCanceledException e)
            {
                var player = new Player(myMainWindow.DisplayWindow);
                player.Display("Ask sensor cancelled.");
            }
        }

        public void StartAskSensor()
        {
            cancelAskSensor = new CancellationTokenSource();
            InitializeAskSensorTask();
        }

        public void OnAskSensorTick()
        {
            var byteArray = GenerateRandomByteArray();

            msgQueue.Add(byteArray);
        }

        private byte[] GenerateRandomByteArray()
        {
            byte[] result = new byte[3];
            var randomNum = new Random();
            for (int i = 0; i < 3; i++)
            {
                result[i] = Convert.ToByte(randomNum.Next(0, 9));
            }

            return result;
        }

        private void GotMsg()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Got Msg Success!");
        }

        private void SentSuccess()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Sent Msg Success!");
        }

        public void Send(byte[] msg)
        {
            msgQueue.Add(msg);
        }

        private void SendFailed(byte[] msg)
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Sent Msg Failed!");

            //MySerialPort.Close();
            //MySerialPort.Dispose();
            /*     MySerialPort = new SerialPort(MyPortName, MyBaudRate, MyParity, MyDataBits, MyStopBits)
                 {
                     ReadTimeout = MyReadTimeout,
                     WriteTimeout = MyWriteTimeout,
                     DtrEnable = true,
                     RtsEnable = true
                 };

                 if (!MySerialPort.IsOpen)
                 {
                     MySerialPort.Open();
                 }*/

            //MySerialPort.DiscardInBuffer();
            //MySerialPort.DiscardOutBuffer();
        }

        public void StopAskSensor()
        {
            cancelAskSensor.Cancel();
        }
    }
}
