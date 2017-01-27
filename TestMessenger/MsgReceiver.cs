using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TestMessenger
{
    class MsgReceiver: Msgr
    {
        public MsgReceiver(SerialPort myPort, MainWindow mw) : base(myPort, mw)
        {
            myPort.DataReceived += Receive;
        }

        public void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            var length = Port.BytesToRead;

            var msggot = ReadAllBytes(Port, length);

            var player = new Player(MyMainWindow.MsgGot);
            var msgStr = GenerateStringFromByteArray(msggot);
            player.Display(msgStr);

            //done, unsub from port
            Port.DiscardInBuffer();
            Port.DataReceived -= Receive;

            SendAck();
            OnMsgrDone();
        }

        private byte[] ReadAllBytes(SerialPort port, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Convert.ToByte(Port.ReadByte());
            }
            return result;
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

        private void SendAck()
        {
            byte[] msg = new byte[1];
            msg[0] = Cmd.AckReceiveOk;
            SendMsg(msg);

            var player = new Player(MyMainWindow.AckSent);
            player.Display(Cmd.AckReceiveOk.ToString());
        }
    }
}
