using System;
using System.Collections;

namespace Lovatto.Chicas.Internal
{
    public partial class EndianBinaryWriter : IDisposable
    {
        /// <summary>
        ///     Writes a object to the stream, using the <see cref="NetworkSerializer"/> for this writer.
        /// </summary>
        /// <param name="value">The value to write. Must not be null.</param>
        /// <exception cref="ArgumentNullException">value is null</exception>
        public void Write(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            CheckDisposed();

            var type = SerializationUtility.GetObjectType(value.GetType());
            Write((byte)type);

            switch (type)
            {
                case SerializeObjectType.Byte:
                    Write((byte)value);
                    break;

                case SerializeObjectType.Short:
                    Write((short)value);
                    break;

                case SerializeObjectType.Integer:
                    Write((int)value);
                    break;

                case SerializeObjectType.Float:
                    Write((float)value);
                    break;

                case SerializeObjectType.String:
                    Write((string)value);
                    break;

                case SerializeObjectType.IntArray:
                    Write((int[])value);
                    break;

                case SerializeObjectType.ObjectArray:
                    Write((object[])value);
                    break;

                case SerializeObjectType.ByteArray:
                    Write((short)((byte[])value).Length);
                    Write((byte[])value);
                    break;

                case SerializeObjectType.GameRoom:
                    Write((GameRoom)value);
                    break;

                default:
                    throw new ArgumentNullException($"No implemented serialization type: {type.ToString()}");
            }
        }

        /// <summary>
        ///     Writes a object array to the stream, using the <see cref="NetworkSerializer"/> for this writer.
        /// </summary>
        /// <param name="value">The value to write. Must not be null.</param>
        /// <exception cref="ArgumentNullException">value is null</exception>
        public void Write(object[] value)
        {
            if (value == null)
                value = new object[0];

            CheckDisposed();

            short length = (short)value.Length;
            Write(length);

            for (int i = 0; i < length; i++)
            {
                Write(value[i]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Write(IDictionary value)
        {
            if (value == null) value = default(IDictionary);

            CheckDisposed();

            short length = (short)value.Count;
            Write(length);

            ArrayList keys = new ArrayList(value.Keys);
            ArrayList values = new ArrayList(value.Values);

            for (int i = 0; i < length; i++)
            {
                Write(keys[i]);
                Write(values[i]);
            }
        }

        /// <summary>
        ///     Writes a object to the stream, using the <see cref="NetworkSerializer"/> for this writer.
        /// </summary>
        /// <param name="value">The value to write. Must not be null.</param>
        /// <exception cref="ArgumentNullException">value is null</exception>
        public void Write(GameRoom value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            CheckDisposed();

            byte[] data = NetworkSerializer.SerializeStream(value);
            Write((short)data.Length);
            Write(data);
        }
    }
}