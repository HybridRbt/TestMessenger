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

        public event MsgReceiver.MsgrDoneEventHandler MsgrDone;

        protected virtual void OnMsgrDone()
        {
            MsgrDone?.Invoke();
        }

        public Msgr(SerialPort myPort, MainWindow mainWindow)
        {
            Port = myPort;
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

                Port.DiscardOutBuffer();
            }
            else
            {
                Console.WriteLine(comPortIsNotOpenMsg);
            }
        }
    }
}