using Lovatto.Chicas.Internal;
using Telepathy;

namespace Lovatto.Chicas
{
    public class InvitationHandler
    {
        public static void HandleSendInvitation(ClientRef connectionID, ChicasPacket packet)
        {
            var message = new NetworkMessage();
            packet.DeserializeBuffer(ref message);

            int friendConnectionID = (int)message.GetParam("id");
            var friend = ChicasSocket.Active.GetClient(friendConnectionID);

            if (friend == null)
            {
                Log.Warning($"Couldn't found client with conID {friendConnectionID}");
                return;
            }

            var sender = connectionID.Client;
            if (sender == null)
            {
                Log.Warning($"Couldn't found client sender with conID {connectionID}");
                return;
            }
            sender.ProvidedAddress = (string)message.GetParam("ip");

            string data = $"{sender.NickName}|{connectionID}|{sender.AuthID}|{sender.ProvidedAddress}|{(int)message.GetParam("port")}";

            packet.Code = (short)ChicasInternalEventType.ReceiveInvitation;
            packet.SetBinary(NetworkSerializer.SerializeText(data));

            ChicasSocket.SendData(friendConnectionID, packet.GetSerializedPacket());
            Log.Info($"Invitation send to {friend.NickName} from {sender.NickName} to join in {sender.ProvidedAddress}");
        }
    }
}
