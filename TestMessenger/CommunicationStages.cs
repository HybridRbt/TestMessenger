using System.Security.RightsManagement;

namespace TestMessenger
{
    public static class CommunicationStages
    {
        public enum States
        {
            Standby,
            WaitEot,
            WaitContent,
            WaitAck
        }

        public enum Triggers
        {
            SentEnq,
            SentEot,
            SentContent,
            SentAck,
            GotAck
        }

        public enum Status
        {
            Normal,
            Failed1,
            Failed2,
            Failed3
        }

        public static Triggers LastTrigger { get; set; }


    }
}