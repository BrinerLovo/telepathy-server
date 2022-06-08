using System.Collections;

namespace Lovatto.Chicas.Internal
{
    public static class ChicasNetworkingUtility 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] InsertInitialByteInArray(short firstByte, byte[] sourceArray)
        {
            byte[] newArray = new byte[sourceArray.Length + 2];
            sourceArray.CopyTo(newArray, 2);
            EndianBitConverter.Big.CopyBytes(firstByte, newArray, 0);
            return newArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        internal static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }
}