using Lovatto.Chicas.Internal;
using System;
using System.Runtime.CompilerServices;
using Telepathy;

namespace Lovatto.Chicas
{
    [Serializable]
    public struct PlayerRef
    {
        private int _index;
        private ChicasClient _player;

        /// <summary>
		/// Player id this player ref represents
		/// </summary>
		public int ConnectionId
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Player data
        /// </summary>
        public ChicasClient Player
        {
            get
            {
                if (_player == null) _player = GameServer.I.GetClient(this);
                return _player;
            }
        }

        /// <summary>
        /// Send serialized data to this client.
        /// </summary>
        /// <param name="data"></param>
        public void Writte(ArraySegmentX<byte> data)
        {
            ServerConsole.ServerSendToSingle(this, data);
        }

        /// <summary>
        /// Send data to this client
        /// This automatically serialize the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="eventType"></param>
        public void SerializeAndWritte(byte[] data, ChicasInternalEventType eventType)
        {
            var package = ChicasPacketPool.AcquirePacket();
            package.SetCode(eventType)
                .SetBinary(data);

            ServerConsole.ServerSendToSingle(this, package.GetSerializedSegment());
        }

        public override bool Equals(object obj)
        {
            if (obj is PlayerRef)
            {
                PlayerRef playerRef = (PlayerRef)obj;
                return _index == playerRef._index;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return _index;
        }

        public override string ToString()
        {
            if (_index <= 0)
            {
                return "[Player:None]";
            }
            return string.Format("[Player:{0}]", _index - 1);
        }

        public static implicit operator PlayerRef(int value)
        {
            PlayerRef result = default(PlayerRef);
            result._index = value + 1;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(PlayerRef value)
        {
            return value._index > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(PlayerRef value)
        {
            return value._index - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(PlayerRef a, PlayerRef b)
        {
            return a._index == b._index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(PlayerRef a, PlayerRef b)
        {
            return a._index != b._index;
        }
    }
}
