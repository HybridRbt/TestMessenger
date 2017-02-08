using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace TestMessenger
{
    /// <summary>
    /// </summary>
    public class MessageSender : IMessageSender
    {
        /// <summary>
        /// </summary>
        public SerialPort Port { get; set; }

        /// <summary>
        /// </summary>
        protected MainWindow MyMainWindow;

        /// <summary>
        /// </summary>
        public delegate void MessageIsSentEventHandler();

        /// <summary>
        /// </summary>
        public event MessageIsSentEventHandler MessageIsSent;

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        public delegate void SendMessageFailedEventHandler(byte[] msg);

        /// <summary>
        /// </summary>
        public event SendMessageFailedEventHandler SendMessageFailed;

        /// <summary>
        /// </summary>
        protected virtual void OnMsgrDone()
        {
            MessageIsSent?.Invoke();
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnSendMessageFailed(byte[] msg)
        {
            SendMessageFailed?.Invoke(msg);
        }

        /// <summary>
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="mainWindow"></param>
        public MessageSender(CommuManager cm, MainWindow mainWindow)
        {
            Port = cm.MySerialPort;
            MyMainWindow = mainWindow;
        }

        /// <summary>
        /// </summary>
        /// <param name="msgToSend"></param>
        public void SendMessage(byte[] msgToSend)
        {
            const string comPortIsNotOpenMsg = "COM Port is not open!";

            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (UnauthorizedAccessException unauthorizedAccessException)
                {
                }
                catch (IOException ioException)
                {
                    // TODO: Handle the IOException 
                }
                catch (ArgumentOutOfRangeException argumentOutOfRangeException)
                {
                    // TODO: Handle the ArgumentOutOfRangeException 
                }
                catch (ArgumentException argumentException)
                {
                    // TODO: Handle the ArgumentException 
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    // TODO: Handle the InvalidOperationException 
                }
            }

            if (Port.IsOpen)
            {
                try
                {
                    Port.Write(msgToSend, 0, msgToSend.Length);
                }
                catch (ArgumentNullException argumentNullException)
                {
                    // TODO: Handle the ArgumentNullException 
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    // TODO: Handle the InvalidOperationException 
                }
                catch (ArgumentException argumentException)
                {
                    // TODO: Handle the ArgumentException 
                }
                catch (TimeoutException timeoutException)
                {
                    // TODO: Handle the TimeoutException 
                }
            }
            else
            {
                Console.WriteLine(comPortIsNotOpenMsg);
            }
        }
    }
}