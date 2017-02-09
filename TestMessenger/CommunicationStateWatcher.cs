using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace TestMessenger
{
    public class CommunicationStateWatcher
    {
        public delegate void StateTimeoutEventHandler();

        public event StateTimeoutEventHandler StateTimeout;

        public CommunicationStages.Status CurrentStatus { get; private set; }

        public CommunicationStages.States CurrentState { get; private set; }

        private Timer StateTimer;

        public CommunicationStateWatcher()
        {
            CurrentState = CommunicationStages.States.Standby;
            CurrentStatus = CommunicationStages.Status.Normal;

            StateTimer = new Timer(1000) {AutoReset = false};
            StateTimer.Elapsed += HandleTimeout;
        }

        private void HandleTimeout(object sender, ElapsedEventArgs e)
        {
            //MessageBox.Show($"State Time out at {CurrentState}!");
            switch (CurrentStatus)
            {
                case CommunicationStages.Status.Normal:
                    CurrentStatus = CommunicationStages.Status.Failed1;
                    ResetStateTimer();
                    
                    break;
                case CommunicationStages.Status.Failed1:
                    CurrentStatus = CommunicationStages.Status.Failed2;
                    ResetStateTimer();
                    
                    break;
                case CommunicationStages.Status.Failed2:
                    CurrentStatus = CommunicationStages.Status.Failed3;
                    ResetStateTimer();
                    
                    break;
                case CommunicationStages.Status.Failed3:  // final state, no more retry
                    CurrentStatus = CommunicationStages.Status.Normal;
                    CurrentState = CommunicationStages.States.Standby;
                    StateTimer.Stop();
                    
                    break;
            }

            OnStateTimeout();
        }

        public bool CanSend()
        {
            return CurrentState == CommunicationStages.States.Standby;
        }

        public void ChangeState(CommunicationStages.Triggers trigger)
        {
            switch (CurrentState)
            {
                 case CommunicationStages.States.Standby:  
                    if (trigger == CommunicationStages.Triggers.SentEnq) // send
                    {
                        CurrentState = CommunicationStages.States.WaitEot;
                        StateTimer.Start();
                    }
                    else if (trigger == CommunicationStages.Triggers.SentEot) // receive
                    {
                        CurrentState = CommunicationStages.States.WaitContent;
                        StateTimer.Start();
                    }

                    break;
                case CommunicationStages.States.WaitEot:  // send
                    if (trigger == CommunicationStages.Triggers.SentContent)
                    {
                        CurrentState = CommunicationStages.States.WaitAck;
                        ResetStateTimer();
                    }

                    break;
                case CommunicationStages.States.WaitAck:   // send
                    if (trigger == CommunicationStages.Triggers.GotAck)
                    {
                        CurrentState = CommunicationStages.States.Standby;
                        StateTimer.Stop();
                    }

                    break;
                case CommunicationStages.States.WaitContent: // receive
                    if (trigger == CommunicationStages.Triggers.SentAck)
                    {
                        CurrentState = CommunicationStages.States.Standby;
                        StateTimer.Stop();
                    }

                    break;
            }   
        }

        private void ResetStateTimer()
        {
            StateTimer.Stop();
            StateTimer.Start();
        }

        protected virtual void OnStateTimeout()
        {
            StateTimeout?.Invoke();
        }
    }
}
