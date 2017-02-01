using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace TestMessenger
{
    class MsgSender: Msgr
    {
        private byte[] myMsg;

        public MsgSender(CommuManager cm, byte[] msg, MainWindow mw) : base(cm, mw)
        {
            myMsg = msg;   
        }

        public void SendMsg()
        {
            SendMsg(myMsg);
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
