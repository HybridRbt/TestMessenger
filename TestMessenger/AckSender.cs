using System;
using System.IO.Ports;

namespace TestMessenger
{
    class AckSender: MessageSender
    {
        public AckSender(CommuManager cm, MainWindow mw) : base(cm, mw)
        {
        }

        public void SendAck()
        {
            byte[] msg = new byte[1];
            msg[0] = Command.AckReceiveOk;
            SendMessage(msg);

            var player = new Player(MyMainWindow.AckSent);
            player.Display(Command.AckReceiveOk.ToString());
        }
    }
}
