using System.Collections;
using System;
using System.Collections.Generic;
using Telepathy;

namespace Lovatto.Chicas.Internal
{
    public class ChicasPacketPool
    {
        private readonly Stack<ChicasPacket> pool = new Stack<ChicasPacket>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        internal void Release(ChicasPacket packet)
        {
            lock (pool)
            {
                packet.Length = 0;
                packet.Pointer = 0;
                packet.IsPooled = true;
                pool.Push(packet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ChicasPacket Acquire()
        {
            ChicasPacket udpPacket = null;
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    udpPacket = pool.Pop();
                }
            }
            if (udpPacket == null)
            {
                udpPacket = new ChicasPacket(0, this);
            }

            udpPacket.IsPooled = false;
            udpPacket.Pointer = 0;
            udpPacket.Length = 0;
            return udpPacket;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ChicasPacket Acquire(byte[] buffer, int offset)
        {
            var packet = Acquire();
            try
            {
                var converter = EndianBitConverter.Big;
                var flags = buffer[offset];
                var opCode = converter.ToInt16(buffer, offset + 1);
                var pointer = offset + 3;

                var dataLength = converter.ToInt32(buffer, pointer);
                pointer += 4;

                // @TODO: Implement nonalloc byte array
                var data = new byte[dataLength];
                Array.Copy(buffer, pointer, data, 0, dataLength);
                pointer += dataLength;

                packet.SetBinary(data);
                packet.Pointer = pointer;
                packet.Code = opCode;
                packet.Flag = flags;

                /* var message = new IncommingMessage(opCode, flags, data, DeliveryMethod.Reliable, peer)
                 {
                     SequenceChannel = 0
                 };*/

            }
            catch (Exception e)
            {
                Log.Error("WS Failed parsing an incoming packet " + e);
            }
            return packet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ChicasPacket AcquirePacket()
        {
            return ChicasSocket.Active.packetPool.Acquire();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Free()
        {
            lock (pool)
            {
                while (pool.Count > 0)
                {
                    pool.Pop();
                }
            }
        }
    }
}
