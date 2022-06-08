using System;
using Telepathy;

namespace Lovatto.Chicas.Internal
{
    [Serializable]
    public class ChicasPacket : IDisposable
    {
        private readonly ChicasPacketPool Pool;
        internal byte[] Data;
        internal int Ptr;
        internal bool IsPooled = true;
        internal int RawDataLength;

        /// <summary>
        /// Message type identifier.
        /// </summary>
        public short Code
        {
            get;
            set;
        }

        /// <summary>
        /// Raw byte array
        /// Doesn't contain the package headers.
        /// </summary>
        public byte[] Buffer
        {
            get => Data;
            private set => Data = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Pointer
        {
            get => Ptr;
            set
            {
                Ptr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        public byte Flag
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public ChicasPacket(short code, ChicasPacketPool pool)
        {
            Code = code;
            Pool = pool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public ChicasPacket(short code, byte[] data, ChicasPacketPool pool)
        {
            Code = code;
            Pool = pool;
            SetBinary(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ChicasPacket SetBinary(byte[] data)
        {
            Data = data;
            RawDataLength = data.Length;
            Length = RawDataLength
                + 2  // Code
                + 1  // Flag
                + 4; // Data Length

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        public ChicasPacket SetCode(ChicasInternalEventType eventType)
        {
            Code = (short)eventType;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetSerializedPacket()
        {
            BigEndianBitConverter converter = EndianBitConverter.Big;
            // @TODO: Implement nonallow byte array.
            byte[] packet = new byte[Length];

            Pointer = 0;
            packet[0] = Flag;
            Pointer++;

            // write the packet code header
            converter.CopyBytes(Code, packet, Pointer);
            Pointer += 2;

            // write data length header
            converter.CopyBytes(RawDataLength, packet, Pointer);
            Pointer += 4;

            // write the packet data
            Array.Copy(Data, 0, packet, Pointer, RawDataLength);
            Pointer += RawDataLength;

            return packet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        public void DeserializeBuffer<T>(ref T container) where T : ICustomSerializable
        {
           NetworkSerializer.DeserializeStream(Buffer, ref container);
        }

        /// <summary>
        /// Get the serialized packaged (including headers) as <see cref="ArraySegment{T}"/>
        /// </summary>
        /// <returns></returns>
        public ArraySegmentX<byte> GetSerializedSegment()
        {
            return new ArraySegmentX<byte>(GetSerializedPacket());
        }

        /// <summary>
        /// Get the package buffer (without headers) as <see cref="ArraySegment{T}"/>
        /// </summary>
        /// <returns></returns>
        public ArraySegmentX<byte> GetBufferSegment()
        {
            return new ArraySegmentX<byte>(Buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
           if(Pool != null)
            {
                Pool.Release(this);
            }
        }
    }
}