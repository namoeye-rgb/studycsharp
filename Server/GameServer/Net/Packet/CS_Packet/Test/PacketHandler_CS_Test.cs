using GameServer.Manager;
using PacketLib.Message;

namespace GameServer.Packet
{
    public partial class PacketHandler_CS
    {
        public static void OnPacketReceive(ulong clientGuid, PK_CS_REQ_LOGIN pks)
        {
            //var testSender = SessionMgr.Instance.GetSession(clientGuid, out Net.Session.ClientSession session);

            //var packet = new PK_CS_RES_LOGIN();
            //packet.TestStr = "TEST";

            //session.Send(packet);
        }
    }
}
