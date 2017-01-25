using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace messenger
{
    public static class Cmd
    {
        public const int EnqReadySend = 0x05; //00000101
        public const int EotReadyReceive = 0x04; //00000100
        public const int AckReceiveOk = 0x06; //00000110
        public const int NakReceiveFail = 0x15; //00010101
    }
}
