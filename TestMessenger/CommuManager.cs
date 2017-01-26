using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

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

        public CommuManager(string port, MainWindow mainWindow)
        {
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
