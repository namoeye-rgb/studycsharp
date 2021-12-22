using LobbyServer.Manager;
using LobbyServer.Net;
using PacketLib.Message;
using System;
using UtilLib.Log;

namespace LobbyServer.Packet
{
    public partial class PacketHandler_S2S
    {
        public static void PacketReceive(PK_S2S_RES_CONNECT _pks)
        {
            S2SClient.Instance.S2SClientNumber = (ushort)_pks.Id;

            Log.Instance.Debug($"PacketReceive LobbyServer PK_S2S_RES_CONNECT, {_pks.Id}");

        }
    }
}
