using NetLib;
using NetLib.Peer;
using PacketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Net.Session
{
    public class S2SSession
    {
        //private readonly TcpSession session;

        //public delegate void OnS2SDisConnect(ushort _id);
        //public OnS2SDisConnect OnDisConnectCallBack;

        //public ushort S2SId { get; set; }

        //public S2SSession(Socket _socket, ushort _id)
        //{
        //    S2SId = _id;
        //    session = new TcpSession(_socket);
        //    session.OnDisConnectCallBack = OnDisConnect;
        //}

        //public void Send<TPacket>(TPacket _packet) where TPacket : PK_BASE
        //{
        //    var idBytes = BitConverter.GetBytes(S2SId);
        //    var buffer = Serializer.Serialize(_packet);
            
        //    //session.Send
        //}
        
        //public void OnDisConnect()
        //{
        //    OnDisConnectCallBack?.Invoke(S2SId);
        //}

        //public void Close()
        //{
        //    session.Close();
        //}
    }
}
