using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace TestMessenger
{
    class Msgr: IMsgr
    {
        public SerialPort Port { get; set; }
        protected MainWindow MyMainWindow;

        public delegate void MsgrDoneEventHandler();

        public event MsgrDoneEventHandler MsgrDone;

        public delegate void MsgrFailedEventHandler(byte[] msg);

        public event MsgrFailedEventHandler MsgrFailed;

        protected virtual void OnMsgrDone()
        {
            MsgrDone?.Invoke();
        }

        protected virtual void OnMsgrFailed(byte[] msg)
        {
            MsgrFailed?.Invoke(msg);
        }

        public Msgr(CommuManager cm, MainWindow mainWindow)
        {
            Port = cm.MySerialPort;
            MyMainWindow = mainWindow;
        }

        public void SendMsg(byte[] msgToSend)
        {
            const string comPortIsNotOpenMsg = "COM Port is not open!";

            if (!Port.IsOpen)
            {
                Port.Open();
            }

            if (Port.IsOpen)
            {
                try
                {
                    Port.Write(msgToSend, 0, msgToSend.Length);
                }
                catch (ArgumentOutOfRangeException timeoutException)
                {
                    Console.WriteLine(timeoutException.Message);
                    return;
                }

                //Port.DiscardOutBuffer();
            }
            else
            {
                Console.WriteLine(comPortIsNotOpenMsg);
            }
        }
    }
}