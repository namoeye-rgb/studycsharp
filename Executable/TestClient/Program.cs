using NetLib;
using NetLib.Token;
using PacketLib;
using PacketLib.Message;
using System;
using System.Net.Sockets;

namespace ConsoleApp2
{
    public class TestTcp : IUserToken
    {
        public TestTcp()
        {
            //Tcp Test
            //Packet
            PacketLib.ZeroFormatter.PacketListInitializer.Init();

            networkTcpClient.OnConnect = ConnectNetToken;
            networkTcpClient.OnReceive = ReceiveBuffer;
            networkTcpClient.OnDisConnect = DisConnect;

            networkTcpClient.Init_Client();

            packetHandler = new PacketHandler();
            packetHandler.SetHandler(typeof(Test), "PacketReceive", (x, c, m) => {
                var pksId = PacketList.Get(x[0].ParameterType);
                if (pksId == 0) {
                    return;
                }

                c.Add(pksId, m);
            });

            PacketParser.InitParseMethod();
        }

        private NetworkHandler networkTcpClient = new NetworkHandler(NETCLIENT_TYPE.CS);
        public PacketHandler packetHandler;
        private NetToken netToken;

        public void Connect(string _ip, int _port, bool _isReconnect)
        {
            networkTcpClient.Start_Client(_ip, _port, _isReconnect);
        }

        public void ReceiveBuffer(IUserToken _userToken, byte[] _buffer)
        {
            Deserializer.Deserialize_Server(_buffer, out PK_BASE packet, out ushort packetId);
            packetHandler.DispatchPacket(packetId, packet);
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var packetBuffer = Serializer.Serialize_Server(_packet);
            networkTcpClient.SendPacket(packetBuffer);
        }

        private void ConnectNetToken(SocketError _socketState, NetToken _netToken)
        {
            Console.WriteLine(_socketState);
            if (_socketState != SocketError.Success)
            {
                return;
            }
            _netToken.SetUserToken(this);
        }

        public void DisConnect(NetToken _netToken)
        {
            Console.WriteLine("Server Disconnect");
        }

        public void Close()
        {
            networkTcpClient.Close();
        }
    }

    internal class Program
    {

        //public static RakClient rakClient;

        public static TestTcp tcp = new TestTcp();

        private static void Main(string[] args)
        {
            tcp.Connect("127.0.0.1", 4000, false);

            while (true)
            {

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 's')
                    {
                        var packet = new PK_CS_LOGIN();
                        packet.AuthKey = "ab333c3";

                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'c')
                    {
                        var packet = new PK_CS_CREATE_USER();
                        packet.AuthKey = "ab333c3";
                        packet.NickName = "t333est3";

                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.E)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'e')
                    {
                        var packet = new PK_CS_CHARACTER_INVEN_LIST();
                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.B)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'b')
                    {
                        var packet = new PK_CS_MAIL_LIST();
                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.D)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'd')
                    {
                        var packet = new PK_CS_USER_DECK_INFO();
                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.W)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'w')
                    {
                        var packet = new PK_CS_USER_DECK_SAVE();
                        packet.DeckList = new System.Collections.Generic.List<string>();
                        packet.DeckGroupId = 0;

                        for(int i = 0; i < 20; ++i)
                        {
                            packet.DeckList.Add(string.Empty);
                        }

                        packet.DeckList[10] = "5e02d9783f0ce5b7f825d349";

                        tcp.SendPacket(packet);
                    }
                }

                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.R)
                {
                    var k = Console.ReadKey();
                    if (k.KeyChar == 'r')
                    {
                        var packet = new PK_CS_MAIL_REWARD();
                        packet.MailUID = "5dc70c3c3f0ce5750c3a7ae0";
                        tcp.SendPacket(packet);
                    }
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
        public static void PacketReceive(PK_SC_LOGIN pks)
        {
            Console.WriteLine(pks);

            if (pks.ErrorCode == GameCommon.Enum.ERROR_CODE.ACCOUNT_NOT_EXISTS)
            {

                var packet_create = new PK_CS_CREATE_USER();
                packet_create.AuthKey = "ab333c3";
                packet_create.NickName = "t333est3";

                Program.tcp.SendPacket(packet_create);
            }

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_CREATE_USER pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_CHARACTER_INVEN_LIST pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_MAIL_LIST pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_MAIL_REWARD pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_USER_DECK_INFO pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }

        public static void PacketReceive(PK_SC_USER_DECK_SAVE pks)
        {
            Console.WriteLine(pks);

            //Console.WriteLine(pks.TestStr);
            //var packet = new PK_CS_REQ_LOGIN();
            //packet.TestStr = "TEST2";

            //Program.rakClient.Send(packet);
        }
    }
}
