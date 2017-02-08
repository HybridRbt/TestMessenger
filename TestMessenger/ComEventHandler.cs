using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMessenger
{
    class ComEventHandler
    {
        private CommuManager myCm;
        private SerialPort myPort;
        private MainWindow myMw;

        public delegate void GotEnqEventHandler();

        public event GotEnqEventHandler GotEnq;

        public delegate void GotEotEventHandler();

        public event GotEotEventHandler GotEot;

        public delegate void GotAckEventHandler();

        public event GotAckEventHandler GotAck;

        public delegate void GotMsgEventHandler(byte[] msgGot);

        public event GotMsgEventHandler GotMsg;

        /// <summary>
        /// </summary>
        public byte[] MsgReceived { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="mw"></param>
        public ComEventHandler(CommuManager cm, MainWindow mw)
        {
            myCm = cm;
            myPort = myCm.MySerialPort;
            myPort.DataReceived += Receive;
            myMw = mw;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            var length = myPort.BytesToRead;
            Player player;

            if (length == 1) //it's ENQ/EOT/ACK
            {
                var msggot = myPort.ReadByte();

                switch (msggot)
                {
                    case Cmd.EnqReadySend:
                        player = new Player(myMw.EnqGot);
                        player.Display(Cmd.EnqReadySend.ToString());
                        OnGotEnq();
                        break;
                    case Cmd.EotReadyReceive:
                        player = new Player(myMw.EotGot);
                        player.Display(Cmd.EotReadyReceive.ToString());
                        OnGotEot();
                        break;
                    case Cmd.AckReceiveOk:
                        player = new Player(myMw.AckGot);
                        player.Display(Cmd.AckReceiveOk.ToString());
                        OnGotAck();
                        break;
                }
                return;
            }

            player = new Player(myMw.MsgGot);
            MsgReceived = new byte[length];
            myPort.Read(MsgReceived, 0, length);
            player.Display(myCm.GenerateStringFromByteArray(MsgReceived));
            OnGotMsg();
        }

        protected virtual void OnGotEnq()
        {
            GotEnq?.Invoke();
        }

        protected virtual void OnGotEot()
        {
            GotEot?.Invoke();
        }

        protected virtual void OnGotAck()
        {
            GotAck?.Invoke();
        }

        protected virtual void OnGotMsg(byte[] msggot)
        {
            GotMsg?.Invoke(msggot);
        }
    }
}
