using System;

namespace Lovatto.Chicas
{
    public static class ChicasNetworkingExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[] { item };
            }

            T[] result = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }

            result[array.Length] = item;
            return result;
        }

        public static T[] AddFirst<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[1] { item };
            }
            T[] array2 = new T[array.Length + 1];
            Array.Copy(array, 0, array2, 1, array.Length);
            array2[0] = item;
            return array2;
        }
    }
}
