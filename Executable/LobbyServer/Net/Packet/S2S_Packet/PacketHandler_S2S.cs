using LobbyServer.Manager;
using LobbyServer.Net;
using NetLib.Token;
using PacketLib.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.Packet
{
    public partial class PacketHandler_S2S
    {
        public static void OnPacketReceive(IUserToken _userToken, PK_S2S_RES_CONNECT _pks)
        {
            S2SClient.Instance.S2SClientNumber = (ushort)_pks.Id;

            Log.Instance.Debug($"OnPacketReceive LobbyServer PK_S2S_RES_CONNECT, {_pks.Id}");
            
        }
    }
}
