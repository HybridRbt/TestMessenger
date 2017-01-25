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

        private TextBox myTb;

        public CommuManager(string port, TextBox tb)
        {
            myTb = tb;
            this.myTb.Text += "Initializing...\n";
            
            _comState = CommunicationStages.Standby;
            this.myTb.Text += "Current commu stage: " + _comState + "\n";

            MyPortName = port;
            MyBaudRate = 115200;
            MyWriteTimeout = 50;
            MyReadTimeout = 200;
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

            MySerialPort.DataReceived += Receive;

            if (!MySerialPort.IsOpen)
            {
                MySerialPort.Open();
            }
        }

        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            const string errorHappenedMsg = "Error Happened!";
            var length = MySerialPort.BytesToRead;
            //var msggot = MySerialPort.ReadByte();
            //if it's ENQ, create a receiver, delegate port to it
            if (length <= 0)
            {
                MySerialPort.DiscardInBuffer();
                return;
            }

            //Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => this.myTb.Text = "Got ENQ"));
            
            MySerialPort.DataReceived -= Receive;
            var receiver = new Msgr(MySerialPort, this.myTb);
        }
 
        public void Send(byte[] msg)
        {
            var sender = new Msgr(MySerialPort, this.myTb);
            sender.SendMsg(msg);
        }
    }
}
