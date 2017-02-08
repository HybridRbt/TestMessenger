using System;

namespace TestMessenger
{
    /// <summary>
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomByteArray()
        {
            byte[] result = new byte[3];
            var randomNum = new Random();
            for (int i = 0; i < 3; i++)
            {
                result[i] = Convert.ToByte(randomNum.Next(0, 9));
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GenerateStringFromByteArray(byte[] msg)
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