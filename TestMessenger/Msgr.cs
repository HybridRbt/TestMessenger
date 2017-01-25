using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace messenger
{
    using System.Windows;
    using System.Windows.Controls;

    using TestMessenger;

    class Msgr
    {
        private SerialPort _port;

        private TextBox myTb;

        public Msgr(SerialPort myPort, TextBox tb)
        {
            _port = myPort;
            _port.DataReceived += Receive;
            this.myTb = tb;
        }

        public void SendMsg(byte[] msgToSend)
        {
            const string comPortIsNotOpenMsg = "COM Port is not open!";

            if (!_port.IsOpen)
            {
                _port.Open();
            }

            if (_port.IsOpen)
            {
                try
                {
                    _port.Write(msgToSend, 0, msgToSend.Length);
                }
                catch (ArgumentOutOfRangeException timeoutException)
                {
                    Console.WriteLine(timeoutException.Message);
                    return;
                }

                _port.DiscardOutBuffer();
            }
            else
            {
                Console.WriteLine(comPortIsNotOpenMsg);
            }
        }

        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            const string errorHappenedMsg = "Error Happened!";
            var msggot = _port.ReadByte();
            Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => this.myTb.Text += msggot));
            
            /*if (NeedToSendCmd || NeedToAck || NeedToAskResponse)
            {
                var msggot = _port.ReadByte();

                var msgToDisplay = new byte[1];
                msgToDisplay[0] = Convert.ToByte(msggot);

                DisplaySerialIn = DisplayContent(msgToDisplay);
                //      WriteLog("Receive Message", msggot.ToString(CultureInfo.InvariantCulture));

                // Collecting the characters received to 'buffer' (string).
                if (NeedToSendCmd)
                {
                    if (msggot == Cmd.EotReadyReceive)
                    {
                        //clear in buffer only when it's not conflicted
                        _port.DiscardInBuffer(); //clear in buffer?

                        var nextCmd = CmdQueue.Dequeue();
                        SendMsg(nextCmd, true);
                        NeedToAck = true;
                        NeedToSendCmd = false; // ready to send cmd
                        LastCmd = nextCmd;
                        TimerForWaitAckFromSingleBoard.Start();

                        return;
                    }

                    var errorMsg =
                        "Got message other than EOT after sending ENQ. Message got from single board is " + msggot;

                    if (!InIoMap && CurrentOp != Operation.None) //ignore commu error in idle.
                    {
                        MessageBox.Show(errorMsg, errorHappenedMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    TransferLogger.AddLogEntry(new ErrorLogItem(errorMsg));
                    NeedToSendCmd = false;
                    NeedToAck = false;
                    NeedToAskResponse = false;
                    return;
                }
                else if (NeedToAck)
                {
                    _port.DiscardInBuffer(); //clear in buffer?
                    if (msggot == Cmd.AckReceiveOk)
                    {
                        TimerForWaitAckFromSingleBoard.Stop();
                        if (LastCmd[1] == Cmd.Abort || LastCmd[1] == Cmd.Alarm || LastCmd[1] == Cmd.Light ||
                            LastCmd[1] == Cmd.CheckPauseButton || LastCmd[1] == Cmd.Pause || LastCmd[1] == Cmd.InitialData)
                        {
                            NeedToAck = false;
                            switch (LastCmd[1])
                            {
                                case Cmd.CheckPauseButton:
                                    /*                                    if (!IsCycleTesting)
                                                                        {
                                                                            EnableTimerForAskSensor = true;
                                                                        }#1#
                                    break;
                                case Cmd.Abort:
                                    if (UseGem && CurrentGemStatus != GemStatus.Offline)
                                    {
                                        MyGem.ReportEvent(GemEvents.OperationAborted);
                                    }
                                    break;
                                case Cmd.Pause:
                                    NeedDisplayPauseDialog = true;
                                    break;
                            }

                            return;
                        }

                        UpdateCurrentOpAndOutputMessage();

                        NeedToAskResponse = true;
                        NeedToAck = false;
                        TimerForWaitResponseFromSingleBoardAfterCmd.StartTimer(); //start waiting for response
                        //MessageBox.Show("Timer started!");

                        return;
                    }

                    var errorMsg =
                        "Got message other than ACK after sending cmd. Message got from single board is " + msggot;

                    if (!InIoMap)
                    {
                        MessageBox.Show(errorMsg, errorHappenedMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    TransferLogger.AddLogEntry(new ErrorLogItem(errorMsg));
                    NeedToSendCmd = false;
                    NeedToAck = false;
                    NeedToAskResponse = false;
                    return;
                }
                else if (NeedToAskResponse)
                {
                    _port.DiscardInBuffer(); //clear in buffer?
                    if (msggot == Cmd.EnqReadySend)
                    {
                        TimerForWaitResponseFromSingleBoardAfterCmd.StopTimer();
                        //MessageBox.Show("Timer Stopped!");
                        var nextCmd = new byte[1];
                        nextCmd[0] = Cmd.EotReadyReceive;
                        SendMsg(nextCmd, false);
                        NeedToAskResponse = false;
                        EnableTimerForSendCmd = false;
                        TimerForWaitMsgFromSingleBoardAfterSendEot.Start();

                        return;
                    }

                    var errorMsg =
                        "Got message other than ENQ after sending cmd. Message got from single board is " + msggot;
                    if (!InIoMap)
                    {
                        MessageBox.Show(errorMsg, errorHappenedMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    TransferLogger.AddLogEntry(new ErrorLogItem(errorMsg));
                    NeedToSendCmd = false;
                    NeedToAck = false;
                    NeedToAskResponse = false;
                    return;
                }
            }

            TimerForWaitMsgFromSingleBoardAfterSendEot.Stop();
            EnableTimerForSendCmd = false;
            var bytesToRead = _port.BytesToRead; //should be msgGot[1] + 1

            if (bytesToRead == 0) return;

            var msgGot = new byte[bytesToRead]; //count in the length of checksum
            if (bytesToRead != 0 && bytesToRead <= 1)
            {
                EnableTimerForSendCmd = false;
                msgGot[0] = Convert.ToByte(_port.ReadByte());
                _port.DiscardInBuffer(); // clear in buffer?
                if (msgGot[0] == Cmd.EnqReadySend)
                {
                    var nextCmd = new byte[1];
                    nextCmd[0] = Cmd.EotReadyReceive;
                    SendMsg(nextCmd, false);
                    NeedToAskResponse = false;
                }
                else
                {
                    var errorMsg =
                        "Got message other than ENQ from single board while idling. Message got from single board is " + msgGot;
                    if (!InIoMap)
                    {
                        MessageBox.Show(errorMsg, errorHappenedMsg, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    TransferLogger.AddLogEntry(new ErrorLogItem(errorMsg));
                }

                return;
            }

            var lengthToRead = _port.ReadByte(); //get the msg length

            while (bytesToRead < lengthToRead + lengthOfChecksum) //length is less than desired, wait for the whole msg
            {
                Thread.Sleep(10);
                bytesToRead = _port.BytesToRead;
            }

            var tempMsg = new byte[lengthToRead + lengthOfChecksum];
            _port.Read(tempMsg, 0, lengthToRead + lengthOfChecksum);

            msgGot = new byte[lengthOfHeader + lengthToRead + lengthOfChecksum];
            msgGot[0] = Convert.ToByte(lengthToRead);
            Array.Copy(tempMsg, 0, msgGot, 1, lengthToRead + lengthOfChecksum);

            var ackCmd = new byte[1];

            if (!Msg.DoChecksum(lengthToRead, msgGot))
            {
                var checksumIsNotCorrectGotMsg = "Checksum is not correct. Got msg: " + DisplayContent(msgGot);

                MessageBox.Show(checksumIsNotCorrectGotMsg);

                if (InIoMap)
                {
                    IsBusy = false;
                }

                TransferLogger.AddLogEntry(new ErrorLogItem(checksumIsNotCorrectGotMsg));
                _port.DiscardInBuffer(); // clear in buffer?
                ackCmd[0] = Cmd.AckReceiveOk;
                SendMsg(ackCmd, false);
                return;
            }

            _port.DiscardInBuffer(); // clear in buffer?
            MsgQueue.Enqueue(msgGot);

            ackCmd[0] = Cmd.AckReceiveOk;
            SendMsg(ackCmd, false);
        }*/
        }
    }
}