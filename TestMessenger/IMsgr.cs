using System.IO.Ports;

namespace TestMessenger
{
    interface IMsgr
    {
        SerialPort Port { get; set; }
        void SendMsg(byte[] msgToSend);
    }
}
