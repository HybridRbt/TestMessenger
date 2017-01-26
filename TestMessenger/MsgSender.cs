using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace TestMessenger
{
    class MsgSender: Msgr
    {
        public byte[] Msg { get; set; }

        public MsgSender(SerialPort myPort, byte[] msg, MainWindow mw) : base(myPort, mw)
        {
            myPort.DataReceived += Receive;
            Msg = msg;
            SendMsg(msg);
        }

        public void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            var msggot = Port.ReadByte();

            //check for ack
            if (msggot != Cmd.AckReceiveOk)
            {
                //Port.DiscardInBuffer();
                return;
            }

            //if it's ack:
            Port.DiscardInBuffer();
            //display ack
            var player = new Player(MyMainWindow.AckGot);
            player.Display(msggot.ToString());

            player = new Player(MyMainWindow.MsgSent);
            player.Display(Convert.ToString(Msg));

            //unsub from port, report done
            Port.DataReceived -= Receive;
            OnMsgrDone();
        }
    }
}
