using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace TestMessenger
{
    class EotChecker: Msgr
    {
        public byte[] MsgToSend { get; set; }

        public EotChecker(SerialPort myPort, byte[] msg, MainWindow mw) : base(myPort, mw)
        {
            MsgToSend = msg;
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

            if (msggot != Cmd.EotReadyReceive)
            {
                Port.DiscardInBuffer();
                return;
            }

            //if it's Eot:
            Port.DiscardInBuffer();
            //display eot
            var player = new Player(MyMainWindow.EotGot);
            player.Display(msggot.ToString());

            //unsub from port, create msg sender
            Port.DataReceived -= Receive;
            var msgsdr = new MsgSender(Port, MsgToSend, MyMainWindow);
            msgsdr.MsgrDone += MsgSent;
        }

        private void MsgSent()
        {
            OnMsgrDone();
        }
    }
}
