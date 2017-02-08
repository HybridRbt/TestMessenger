﻿using System;
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
            var length = GetMessageLength();
            Player player;

            if (length == 1) //it's ENQ/EOT/ACK
            {
                var msggot = GetMessageContent();

                switch (msggot)
                {
                    case Command.EnqReadySend:
                        player = new Player(myMw.EnqGot);
                        player.Display(Command.EnqReadySend.ToString());
                        OnGotEnq();
                        break;
                    case Command.EotReadyReceive:
                        player = new Player(myMw.EotGot);
                        player.Display(Command.EotReadyReceive.ToString());
                        OnGotEot();
                        break;
                    case Command.AckReceiveOk:
                        player = new Player(myMw.AckGot);
                        player.Display(Command.AckReceiveOk.ToString());
                        OnGotAck();
                        break;
                    default:
                        // TODO: add nak handler
                        break;
                }
                return;
            }

            player = new Player(myMw.MsgGot);
            MsgReceived = new byte[length];
            myPort.Read(MsgReceived, 0, length);
            player.Display(myCm.Helper.GenerateStringFromByteArray(MsgReceived));
            OnGotMsg(MsgReceived);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private int GetMessageContent()
        {
            var msggot = myPort.ReadByte();
            return msggot;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private int GetMessageLength()
        {
            var length = myPort.BytesToRead;
            return length;
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
