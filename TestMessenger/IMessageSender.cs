using System.IO.Ports;

namespace TestMessenger
{
    /// <summary>
    /// </summary>
    internal interface IMessageSender
    {
        /// <summary>
        /// </summary>
        SerialPort Port { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="msgToSend"></param>
        void SendMessage(byte[] msgToSend);
    }
}