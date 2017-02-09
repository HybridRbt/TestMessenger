using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace TestMessenger
{
    using System.Windows.Controls;

    /// <summary>
    /// </summary>
    public class CommuManager
    {
        /// <summary>
        /// </summary>
        public SerialPort MySerialPort { get; set; }

        private string MyPortName { get; set; }

        /// <summary>
        /// </summary>
        public int MyBaudRate { get; set; }

        private int MyWriteTimeout { get; set; }

        private int MyReadTimeout { get; set; }

        private StopBits MyStopBits { get; set; }

        private int MyDataBits { get; set; }

        private Parity MyParity { get; set; }

        public bool EotReplyEnabled { get; set; }

        private MainWindow myMainWindow;
        private TextBox myTb;
        private EnqSender MsgChecker;
        private BlockingCollection<byte[]> msgQueue;

        private ComEventHandler comEventHandler;
        private byte[] _nextMsg;
        private CancellationTokenSource cancelAskSensor;

        private CommunicationStateWatcher stateWatcher;

        /// <summary>
        /// </summary>
        /// <param name="port"></param>
        /// <param name="mainWindow"></param>
        public CommuManager(string port, MainWindow mainWindow)
        {
            msgQueue = new BlockingCollection<byte[]>();
            myMainWindow = mainWindow;

            myTb = mainWindow.DisplayWindow;
            myTb.Text += "Initializing...\n";

            cancelAskSensor = new CancellationTokenSource();

            MyPortName = port;
            MyBaudRate = 115200;
            MyWriteTimeout = 300;
            MyReadTimeout = 300;
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

            stateWatcher = new CommunicationStateWatcher();
            stateWatcher.StateTimeout += HandleStateTimeout;

            var currentState = stateWatcher.CurrentState;
            myTb.Text += "Current commu stage: " + currentState + "\n";

            comEventHandler = new ComEventHandler(this, myMainWindow);
            comEventHandler.GotEnq += SendEot;
            comEventHandler.GotEot += SendContent;
            comEventHandler.GotMsg += ReceiveMsg;
            comEventHandler.GotAck += ReturnToStandby;

            Task.Factory.StartNew(() =>
            {
                if (ComState != CommunicationStages.Standby) return;
                foreach (byte[] msg in msgQueue.GetConsumingEnumerable())
                {
                    SendRequest(msg);
                }
            });
           
            MySerialPort.ErrorReceived += MySerialPort_ErrorReceived;
        }

        /// <summary>
        /// </summary>
        /// <param name="msggot"></param>
        private void ReceiveMsg(byte[] msggot)
        {
            GotMsg();
            var msgStr = Helper.GenerateStringFromByteArray(msggot);
            var player = new Player(myMainWindow.MsgGot);
            player.Display(msgStr);
            SendAck();
        }

        /// <summary>
        /// </summary>
        private void ReturnToStandby()
        {
            SentSuccess();
            ComState = CommunicationStages.Standby;
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        private void SendRequest(byte[] msg)
        {
            _nextMsg = msg;
            var sender = new EnqSender(this, myMainWindow);
            sender.SendEnq();

            // will allow 100 ms for response
            TimeSpan maxDuration = TimeSpan.FromMilliseconds(100);
            Stopwatch sw = Stopwatch.StartNew();

            while (sw.Elapsed < maxDuration)
            {
                // wait for the ComState to change
                if (ComState == CommunicationStages.GotEot)
                    break;
            }

            // timeout 

        }

        /// <summary>
        /// </summary>
        private void SendAck()
        {
            if (!AckReplyEnabled) return;
            var sender = new AckSender(this, myMainWindow);
            sender.SendAck();
        }

        public bool AckReplyEnabled { get; set; }

        /// <summary>
        /// </summary>
        private void SendContent()
        {
            if (!MsgReplyEnabled) return;
            var sender = new MsgSender(this, _nextMsg, myMainWindow);
            sender.SendMsg();
        }

        public bool MsgReplyEnabled { get; set; }

        /// <summary>
        /// </summary>
        private void SendEot()
        {
            if (!EotReplyEnabled) return;
            var sender = new EotSender(this, myMainWindow);
            sender.SendEot();
        }

        private void MySerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The `<paramref name="onTick"/>` method will be called periodically unless canceled.
        /// </summary>
        /// <param name="onTick"></param>
        /// <param name="dueTime"></param>
        /// <param name="interval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private static async Task RunPeriodicAsync(Action onTick,
                                                   TimeSpan dueTime,
                                                   TimeSpan interval,
                                                   CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until canceled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }

        /// <summary>
        /// </summary>
        private async void InitializeAskSensorTask()
        {
            var dueTime = TimeSpan.FromSeconds(1);
            var interval = TimeSpan.FromMilliseconds(500);

            try
            {
                await RunPeriodicAsync(OnAskSensorTick, dueTime, interval, cancelAskSensor.Token);
            }
            catch (OperationCanceledException e)
            {
                var player = new Player(myMainWindow.DisplayWindow);
                player.Display("Ask sensor canceled.");
            }
        }

        /// <summary>
        /// </summary>
        public void StartAskSensor()
        {
            cancelAskSensor = new CancellationTokenSource();
            InitializeAskSensorTask();
        }

        /// <summary>
        /// </summary>
        private void OnAskSensorTick()
        {
            var byteArray = TestMessenger.Helper.GenerateRandomByteArray();

            msgQueue.Add(byteArray);
        }

        /// <summary>
        /// </summary>
        private void GotMsg()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Got Msg Success!");
        }

        /// <summary>
        /// </summary>
        private void SentSuccess()
        {
            var player = new Player(myMainWindow.DisplayWindow);
            player.Display("Sent Msg Success!");
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// </summary>
        public void StopAskSensor()
        {
            cancelAskSensor.Cancel();
        }
    }
}
