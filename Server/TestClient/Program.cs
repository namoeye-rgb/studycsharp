using NetLib;
using NetLib.Token;
using System;
using System.Net.Sockets;

namespace ConsoleApp2
{
    public class TestTcp// : IUserToken
    {
        //public TestTcp()
        //{
        //    //Tcp Test
        //    //Packet

        //    networkTcpClient.OnConnect = ConnectNetToken;
        //    networkTcpClient.OnReceive = ReceiveBuffer;
        //    networkTcpClient.OnDisConnect = DisConnect;

        //    networkTcpClient.Init_Client();

        //    packetHandler = new PacketHandler();
        //    packetHandler.SetHandler(typeof(Test), "PacketReceive", (x, c, m) => {
        //        var pksId = PacketList.Get(x[0].ParameterType);
        //        if (pksId == 0) {
        //            return;
        //        }

        //        c.Add(pksId, m);
        //    });

        //    PacketParser.InitParseMethod();
        //}

        //private NetworkHandler networkTcpClient = new NetworkHandler(NETCLIENT_TYPE.CS);
        //public PacketHandler packetHandler;
        //private NetToken netToken;

        //public void Connect(string _ip, int _port, bool _isReconnect)
        //{
        //    networkTcpClient.Start_Client(_ip, _port, _isReconnect);
        //}

        //public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        //{
        //    Deserializer.Deserialize_Server(_buffer, out PK_BASE packet, out ushort packetId);
        //    packetHandler.DispatchPacket(packetId, packet);
        //}

        //public void SendPacket<T>(T _packet) where T : PK_BASE
        //{
        //    var packetBuffer = Serializer.Serialize_Server(_packet);
        //    networkTcpClient.SendPacket(packetBuffer);
        //}

        //private void ConnectNetToken(SocketError _socketState, NetToken _netToken)
        //{
        //    Console.WriteLine(_socketState);
        //    if (_socketState != SocketError.Success)
        //    {
        //        return;
        //    }
        //    _netToken.SetUserToken(this);
        //}

        //public void DisConnect(NetToken _netToken)
        //{
        //    Console.WriteLine("Server Disconnect");
        //}

        //public void Close()
        //{
        //    networkTcpClient.Close();
        //}
    }

    internal class Program
    {

        //public static RakClient rakClient;

        public static TestTcp tcp = new TestTcp();

        private static void Main(string[] args)
        {
            while (true)
            {

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                {
                    //var k = Console.ReadKey();
                    //if (k.KeyChar == 's')
                    //{
                    //    var packet = new PK_CS_LOGIN();
                    //    packet.AuthKey = "ab333c3";

                    //    tcp.SendPacket(packet);
                    //}
                }

                

                //if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.M)
                //{
                //    var k = Console.ReadKey();
                //    if (k.KeyChar == 'm')
                //    {
                //        var packet = new PK_CS_MATCHING();
                //        packet.MatchingType = MATCHING_ROOM_TYPE.SURVIVE;
                //        tcp.SendPacket(packet);
                //    }
                //}

                //if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.F)
                //{
                //    var k = Console.ReadKey();
                //    if (k.KeyChar == 'f')
                //    {
                //        var packet = new PK_CS_CANCEL_MATCHING();
                //        tcp.SendPacket(packet);
                //    }
                //}

                //if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.D)
                //{
                //    var k = Console.ReadKey();
                //    if (k.KeyChar == 'd')
                //    {
                //        tcp.Close();
                //    }
                //}
            }


            //RakNet Test
            //var rakClient = new RakClient();
            //rakClient.RakNetStartup("127.0.0.1", 5001);

            //PacketLib.ZeroFormatter.PacketListInitializer.Init();

            //rakClient.InitPacketHandler(typeof(RakTest), "PacketReceive");
            //PacketParser.InitParseMethod();

            //rakClient.Connect("127.0.0.1", 5000);

            //while (true) {

            //    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S) {
            //        var k = Console.ReadKey();
            //        if (k.KeyChar == 's') {
            //            var packet = new PK_CS_FIELDOBJECT_MOVE_DIR();
            //            packet.X = 100;
            //            rakClient.SendPacket(packet);
            //        }
            //    }

            //    rakClient.Update();
            //}
        }
    }

    public class Test
    {
        //public static void PacketReceive(PK_SC_LOGIN pks)
        //{
        //}
    }
}
