using System.Collections;

namespace Lovatto.Chicas.Internal
{
    public static class ChicasNetworkingUtility 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static byte[] InsertInitialByteInArray(byte firstByte, byte[] sourceArray)
        {
            byte[] newArray = new byte[sourceArray.Length + 1];
            sourceArray.CopyTo(newArray, 1);
            newArray[0] = firstByte;
            return newArray;
        }
    }
}