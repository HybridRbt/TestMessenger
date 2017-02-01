using System;
using System.IO.Ports;

namespace TestMessenger
{
    class EnqSender: Msgr
    {
        public EnqSender(CommuManager cm, MainWindow mw) : base(cm, mw)
        {
        }

        public void SendEnq()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.EnqReadySend;
            SendMsg(msg);

            var player = new Player(MyMainWindow.EnqSent);
            player.Display(Cmd.EnqReadySend.ToString());
        }
    }
}
