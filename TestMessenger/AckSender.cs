using System;
using System.IO.Ports;

namespace TestMessenger
{
    class AckSender: Msgr
    {
        public AckSender(CommuManager cm, MainWindow mw) : base(cm, mw)
        {
        }

        public void SendAck()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.AckReceiveOk;
            SendMsg(msg);

            var player = new Player(MyMainWindow.AckSent);
            player.Display(Cmd.AckReceiveOk.ToString());
        }
    }
}
