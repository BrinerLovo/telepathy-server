using System;
using System.Collections;
using System.Collections.Generic;

namespace Lovatto.Chicas.Internal
{
    public class SerializationUtility
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SerializeObjectType GetObjectType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return SerializeObjectType.Boolean;

                case TypeCode.Byte:
                    return SerializeObjectType.Byte;

                case TypeCode.Int16:
                    return SerializeObjectType.Short;

                case TypeCode.Int32:
                    return SerializeObjectType.Integer;

                case TypeCode.Int64:
                    return SerializeObjectType.Long;

                case TypeCode.Single:
                    return SerializeObjectType.Float;

                case TypeCode.Double:
                    return SerializeObjectType.Double;

                case TypeCode.String:
                    return SerializeObjectType.String;
            }
            if (type.IsArray)
            {
                if (type == typeof(byte[]))
                {
                    return SerializeObjectType.ByteArray;
                }
                return SerializeObjectType.Array;
            }
            if (type.IsGenericType && (typeof(Dictionary<,>) == type.GetGenericTypeDefinition()))
            {
                return SerializeObjectType.Dictionary;
            }
            if (type == typeof(Lovatto.Chicas.GameRoom))
            {
                return SerializeObjectType.GameRoom;
            }
            return SerializeObjectType.Unknown;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Type GetTypeOfCode(SerializeObjectType type)
        {
            switch (type)
            {
                case SerializeObjectType.StringArray:
                    return typeof(string[]);

                case SerializeObjectType.Byte:
                    return typeof(byte);

                case SerializeObjectType.Double:
                    return typeof(double);

                case SerializeObjectType.Float:
                    return typeof(float);

                case SerializeObjectType.Integer:
                    return typeof(int);

                case SerializeObjectType.Short:
                    return typeof(short);

                case SerializeObjectType.Long:
                    return typeof(long);

                case SerializeObjectType.IntArray:
                    return typeof(int[]);

                case SerializeObjectType.Boolean:
                    return typeof(bool);

                case SerializeObjectType.String:
                    return typeof(string);

                case SerializeObjectType.ByteArray:
                    return typeof(byte[]);

                case SerializeObjectType.Array:
                    return typeof(Array);

                case SerializeObjectType.ObjectArray:
                    return typeof(object[]);

                case SerializeObjectType.Dictionary:
                    return typeof(IDictionary);

                case SerializeObjectType.GameRoom:
                    return typeof(GameRoom);

                default:
                    return typeof(object);
            }
        }
    }
}