using Lovatto.Chicas.Internal;
using System;
using System.Runtime.CompilerServices;
using Telepathy;

namespace Lovatto.Chicas
{
    [Serializable]
    public struct ClientRef
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
                if(_player == null) _player = GameServer.I.GetClient(this);
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
        public void SerializeAndWritte<T>(T data, ChicasInternalEventType eventType)
        {
            ServerConsole.ServerSendToSingle(this, new ArraySegment<byte>(NetworkSerializer.Serialize(data, eventType)));
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientRef)
            {
                ClientRef playerRef = (ClientRef)obj;
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

        public static implicit operator ClientRef(int value)
        {
            ClientRef result = default(ClientRef);
            result._index = value + 1;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(ClientRef value)
        {
            return value._index > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(ClientRef value)
        {
            return value._index - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ClientRef a, ClientRef b)
        {
            return a._index == b._index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ClientRef a, ClientRef b)
        {
            return a._index != b._index;
        }
    }
}
