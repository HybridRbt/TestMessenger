using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace TestMessenger
{
    class EotSender: MessageSender
    {
        private MainWindow myMw;

        public EotSender(CommuManager cm, MainWindow mw) : base(cm, mw)
        {
            myMw = mw;
        }

        public void SendEot()
        {
            var msg = new byte[1];
            msg[0] = Command.EotReadyReceive;
            SendMessage(msg);
            var player = new Player(myMw.EotSent);
            player.Display(Command.EotReadyReceive.ToString());
        }
    }
}
