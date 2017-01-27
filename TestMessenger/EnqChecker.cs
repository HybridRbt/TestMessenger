using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestMessenger
{
    class EnqChecker: Msgr
    {
        public EnqChecker(SerialPort myPort, MainWindow mw) : base(myPort, mw)
        {
            myPort.DataReceived += Receive;
        }

        public void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            int msggot;

            try
            {
                msggot = Port.ReadByte();
            }
            catch (TimeoutException timeoutException)
            {
                Port.DataReceived -= Receive;
                OnMsgrFailed();
                return;
            }

            if (msggot != Cmd.EnqReadySend)
            {
                Port.DiscardInBuffer();
                return;
            }

            //if it's ENQ:
            Port.DiscardInBuffer();
            //display enq
            var player = new Player(MyMainWindow.EnqGot);
            player.Display(msggot.ToString());

            //unsub from port, create msg receiver
            Port.DataReceived -= Receive;
            var msgrcvr = new MsgReceiver(Port, MyMainWindow); 
            msgrcvr.MsgrDone += MsgReceived;
            
            //send eot
            SendEot();
        }

        private void MsgReceived()
        {
            OnMsgrDone();
        }

        private void SendEot()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.EotReadyReceive;
            SendMsg(msg);

            var player = new Player(MyMainWindow.EotSent);
            player.Display(Cmd.EotReadyReceive.ToString());
        }
    }
}
