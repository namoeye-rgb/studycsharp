using GameServer.Net.Session;
using PacketLib.Message;
using System;

namespace GameServer.Packet
{
    public partial class PacketHandler_CG
    {
        public static void PacketReceive(ClientSession _session, PK_CS_FIELDOBJECT_MOVE_DEST pks)
        {

        }

        public static void PacketReceive(ClientSession _session, PK_CS_FIELDOBJECT_MOVE_DIR pks)
        {
            try
            {
                var ppp = new PK_SC_FIELDOBJECT_ENTER();
                ppp.Name = "Test";

                _session.SendPacket(ppp);
            }catch(Exception e)
            {

            }
        }
    }
}
