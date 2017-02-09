namespace TestMessenger
{
    /// <summary>
    /// </summary>
    public static class Command
    {
        /// <summary>
        /// </summary>
        public const int EnqReadySend = 0x05; //00000101

        /// <summary>
        /// </summary>
        public const int EotReadyReceive = 0x04; //00000100

        /// <summary>
        /// </summary>
        public const int AckReceiveOk = 0x06; //00000110

        /// <summary>
        /// </summary>
        public const int NakReceiveFail = 0x15; //00010101
    }
}
