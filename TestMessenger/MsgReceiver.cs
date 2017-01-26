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
            var msggot = Port.ReadExisting();
            var player = new Player(MyMainWindow.MsgGot);
            player.Display(msggot);

            //done, unsub from port
            Port.DataReceived -= Receive;

            SendAck();
            OnMsgrDone();
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
