using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using messenger;

namespace TestMessenger
{
    interface IMsgr
    {
        SerialPort Port { get; set; }
        void SendMsg(byte[] msgToSend);
    }
}
