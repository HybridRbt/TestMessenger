using System;
using System.IO.Ports;

namespace TestMessenger
{
    /// <summary>
    /// </summary>
    internal class EnqSender: MessageSender	
    {
        /// <summary>
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="mw"></param>
        public EnqSender(CommuManager cm, MainWindow mw) : base(cm, mw)
        {
        }

        /// <summary>
        /// </summary>
        public void SendEnq()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.EnqReadySend;
            SendMessage(msg);

            var player = new Player(MyMainWindow.EnqSent);
            player.Display(Cmd.EnqReadySend.ToString());
        }
    }
}
