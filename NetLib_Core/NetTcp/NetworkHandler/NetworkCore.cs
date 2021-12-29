using NetLib.Token;
using System;
using System.Net.Sockets;
using System.Threading;

namespace NetLib
{
    public enum NET_TYPE
    {
        Client,
        Server,
    }

    public partial class NetworkCore
    {
        private INetLogger logger;
        public NET_TYPE NetType { get; }

        //Common
        private AsyncSocket asyncSocket;

        //Client
        private NetToken netToken;
        private int bufferSize = 10;
        private BufferManager bufferMgr;
        private SocketAsyncEventArgsPool receiveEventPool;
        private SocketAsyncEventArgsPool sendEventPool;

        public delegate void OnAccept_CallBack(NetToken netToken);
        public delegate void OnReceive_CallBack(IUserToken userToken, byte[] buffer);
        public delegate void OnConnect_CallBack(SocketError socketState, NetToken netToken);
        public delegate void OnDisConnect_CallBack(NetToken netToken);

        public OnAccept_CallBack OnAccept;
        public OnConnect_CallBack OnConnect;
        public OnReceive_CallBack OnReceive;
        public OnDisConnect_CallBack OnDisConnect;

        public NetworkCore(NET_TYPE type, INetLogger logger = null)
        {
            asyncSocket = new AsyncSocket();
            NetType = type;
            this.logger = logger;
        }


        public void Init_Server(int maxConnection,
            OnAccept_CallBack onAcceptFunc,
            OnConnect_CallBack onConnectFunc,
            OnReceive_CallBack onReceiveFunc,
            OnDisConnect_CallBack onDisConnect)
        {
            //서버에만 풀을 생성해두기 위한것
            InitializePool(maxConnection);
            asyncSocket.AcceptClientCallback = AddClient;
            asyncSocket.ReceiveCompleteCallback = ReceiveComplete;
            asyncSocket.SendCompleteCallback = SendComplete;

            OnAccept += onAcceptFunc;
            OnReceive += onReceiveFunc;
            OnConnect += onConnectFunc;
            OnDisConnect += onDisConnect;
        }

        public void Start_Server(int port, int listenCount = 1000)
        {
            asyncSocket.StartServer(port, listenCount);
        }

        public void Init_Client(OnConnect_CallBack onConnectFunc, OnReceive_CallBack onReceiveFunc, OnDisConnect_CallBack onDisConnect)
        {
            asyncSocket.ServerConnectCallback = ConnectServer;
            asyncSocket.ReceiveCompleteCallback = ReceiveComplete;
            asyncSocket.SendCompleteCallback = SendComplete;

            OnReceive += onReceiveFunc;
            OnConnect += onConnectFunc;
            OnDisConnect += onDisConnect;

        }
        public void Start_Client(string ip, int port, bool isRecoonnect)
        {
            asyncSocket.StartClient(ip, port, isRecoonnect);
        }
        private void InitializePool(int maxConnection)
        {
            bufferMgr = new BufferManager((maxConnection * bufferSize) * 2, bufferSize);

            receiveEventPool = new SocketAsyncEventArgsPool(maxConnection);
            sendEventPool = new SocketAsyncEventArgsPool(maxConnection);

            for (int i = 0; i < maxConnection; ++i)
            {
                NetToken token = new NetToken();
                //recive Pool
                SocketAsyncEventArgs receive = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(receive);
                receive.UserToken = token;
                receive.Completed += asyncSocket.ReceiveComplete;
                receiveEventPool.PushPool(receive);

                //send Pool
                SocketAsyncEventArgs send = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(send);
                send.UserToken = token;
                send.Completed += asyncSocket.SendCompate;
                sendEventPool.PushPool(send);
            }
        }

        #region 소켓내부에서 Complete되고난 뒤에 처리되는 함수
        //서버용
        public void AddClient(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs receiveArgs = receiveEventPool.PopPool();
            SocketAsyncEventArgs sendArgs = sendEventPool.PopPool();

            NetToken token = receiveArgs.UserToken as NetToken;

            AsyncSocket asyncSocket = new AsyncSocket();
            asyncSocket.SetSocket(e.AcceptSocket);
            token.Init(asyncSocket, receiveArgs, sendArgs);

            OnAccept?.Invoke(token);

            //해당 서버 <-> 클라이언트에 연결된 소켓을 리시브해서 대기한다는것임.
            //local에는 서버 remote에는 클라의 주소가 설정된다.
            //Accept을 하면서 remote의 주소가 할당된더고
            //여기서 token을 모으는 이유는 서버에서 클라로 연결을 위함이다.
            token.Receive();

            logger?.Debug("Client Accept : {0}", e.AcceptSocket.RemoteEndPoint);
        }

        //클라이언트 용
        public void ConnectServer(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                logger?.Warn("Fail Connect Server, SocketError : {0}, Remote EndPoint : {1}", e.SocketError, e.RemoteEndPoint);
                Thread.Sleep(10000);

                if (asyncSocket.ReConnectClient() == false)
                {
                    logger?.Error("Not Found Remote Connect Info");
                    return;
                }

                OnConnect?.Invoke(asyncSocket.SocketState, null);

                return;
            }

            Func<SocketAsyncEventArgs> createEventArgsFunc = () =>
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                byte[] buffer = new byte[bufferSize];
                args.SetBuffer(buffer, 0, buffer.Length);

                return args;
            };

            netToken = new NetToken();

            SocketAsyncEventArgs sendArgs = createEventArgsFunc();
            sendArgs.Completed += asyncSocket.SendCompate;
            sendArgs.UserToken = netToken;

            SocketAsyncEventArgs recvArgs = createEventArgsFunc();
            recvArgs.UserToken = netToken;
            recvArgs.Completed += asyncSocket.ReceiveComplete;

            netToken.Init(asyncSocket, recvArgs, sendArgs);
            netToken.Receive();

            OnConnect?.Invoke(asyncSocket.SocketState, netToken);
        }

        private void ReceiveComplete(SocketAsyncEventArgs e)
        {
            var netToken = e.UserToken as NetToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (OnReceive == null)
                {
                    logger?.Error("error not found OnReceive Func");
                    return;
                }

                netToken.ReceiveByteBuffer((token, bytes) =>
                {
                    OnReceive?.Invoke(token, bytes);
                }, e);

                if (netToken.Receive() == false)
                {
                    OnDisConnect?.Invoke(netToken);
                    return;
                }
            }
            else
            {
                OnDisConnect?.Invoke(netToken);
            }
        }

        private void SendComplete(SocketAsyncEventArgs e)
        {
            var data = e.UserToken as NetToken;
            if (data == null)
            {
                Console.WriteLine("error SendComplete NetToken casting");
                return;
            }
            data.SendDequeue();
        }

        #endregion

        //클라이언트 -> 서버 패킷 전송 함수
        public void SendPacket(byte[] sendBuffer)
        {
            netToken.SendPacket(sendBuffer);
        }

        public void Update(double deltaTime)
        {

        }

        public IToken GetNetToken()
        {
            return netToken;
        }

        public void Close()
        {
            netToken?.Close();
        }

        public SocketError GetSocketState()
        {
            if (asyncSocket == null)
            {
                return SocketError.SocketError;
            }

            return asyncSocket.SocketState;
        }
    }
}
