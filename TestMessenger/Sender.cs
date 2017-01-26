using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using messenger;

namespace TestMessenger
{
    class Sender: Msgr
    {
        private byte[] myMsg; 

        public Sender(SerialPort myPort, byte[] msg, MainWindow mw) : base(myPort, mw)
        {
            myMsg = msg;
            
            //create eot checker
            var eotChkr = new EotChecker(myPort, msg, mw);

            //sub to done event
            eotChkr.MsgrDone += MsgSent;

            //send enq
            SendEnq();
        }

        private void MsgSent()
        {
            OnMsgrDone();
        }

        private void SendEnq()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.EnqReadySend;
            var player = new Player(MyMainWindow.EnqSent);
            player.Display(Cmd.EnqReadySend.ToString());
            SendMsg(msg);
        }
    }
}
