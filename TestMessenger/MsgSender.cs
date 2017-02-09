using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace TestMessenger
{
    class MsgSender: MessageSender
    {
        private byte[] myMsg;

        public MsgSender(CommuManager cm, byte[] msg, MainWindow mw) : base(cm, mw)
        {
            myMsg = msg;   
        }

        public void SendMsg()
        {
            SendMessage(myMsg);
            var player = new Player(MyMainWindow.MsgSent);
            var msgStr = GenerateStringFromByteArray(myMsg);
            player.Display(msgStr);
        }

        private string GenerateStringFromByteArray(byte[] msg)
        {
            var result = "";

            for (int i = 0; i < msg.Length; i++)
            {
                result += msg[i].ToString();
            }

            return result;
        }
    }
}
