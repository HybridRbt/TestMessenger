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
            var player = new Player(MyMainWindow.MsgSent);
            var msgStr = GenerateStringFromByteArray(Msg);
            player.Display(msgStr);
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
                return;
            }

            //check for ack
            if (msggot != Cmd.AckReceiveOk)
            {
                Port.DiscardInBuffer();
                return;
            }

            //if it's ack:
            Port.DiscardInBuffer();
            //display ack
            var player = new Player(MyMainWindow.AckGot);
            player.Display(msggot.ToString());

            //unsub from port, report done
            Port.DataReceived -= Receive;
            OnMsgrDone();
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
