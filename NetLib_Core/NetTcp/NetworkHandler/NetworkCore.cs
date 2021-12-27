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
        private AsyncSocket socket;

        //Client
        private NetToken netToken;
        private int bufferSize = 65535;
        private BufferManager bufferMgr;
        private SocketAsyncEventArgsPool receiveEventPool;
        private SocketAsyncEventArgsPool sendEventPool;

        public delegate void OnAccept_CallBack(NetToken _netToken);
        public delegate void OnReceive_CallBack(IUserToken _userToken, byte[] _buffer);
        public delegate void OnConnect_CallBack(SocketError _socketState, NetToken _netToken);
        public delegate void OnDisConnect_CallBack(NetToken _netToken);

        public OnAccept_CallBack OnAccept;
        public OnConnect_CallBack OnConnect;
        public OnReceive_CallBack OnReceive;
        public OnDisConnect_CallBack OnDisConnect;

        public NetworkCore(NET_TYPE _type, INetLogger _logger = null)
        {
            socket = new AsyncSocket();
            NetType = _type;
            logger = _logger;
        }


        public void Init_Server(int _maxConnection,
            OnAccept_CallBack OnAcceptFunc,
            OnReceive_CallBack OnReceiveFunc,
            OnConnect_CallBack onConnectFunc,
            OnDisConnect_CallBack onDisConnect)
        {
            //서버에만 풀을 생성해두기 위한것
            InitializePool(_maxConnection);
            socket.AcceptClient = AddClient;
            socket.ReceiveComplete = ReceiveComplete;
            socket.SendComplete = SendComplete;

            OnAccept += OnAcceptFunc;
            OnReceive += OnReceiveFunc;
            OnConnect += onConnectFunc;
            OnDisConnect += onDisConnect;
        }

        public void Start_Server(int _port, int _listenCount = 1000)
        {
            socket.StartServer(_port, _listenCount);
        }

        public void Init_Client()
        {
            socket.ConnectServer = ConnectServer;
            socket.ReceiveComplete = ReceiveComplete;
            socket.SendComplete = SendComplete;
        }
        public void Start_Client(string _ip, int _port, bool _isRecoonnect)
        {
            socket.StartClient(_ip, _port, _isRecoonnect);
        }
        private void InitializePool(int _maxConnection)
        {
            bufferMgr = new BufferManager(_maxConnection * bufferSize * 2, bufferSize);

            receiveEventPool = new SocketAsyncEventArgsPool(_maxConnection);
            sendEventPool = new SocketAsyncEventArgsPool(_maxConnection);

            for (int i = 0; i < _maxConnection; ++i)
            {
                //recive Pool
                SocketAsyncEventArgs receive = new SocketAsyncEventArgs();
                NetToken token = new NetToken();

                bufferMgr.SetBuffer(receive);

                receive.UserToken = token;
                receive.Completed += socket.ReceiveAsync;
                receiveEventPool.PushPool(receive);

                //send Pool
                SocketAsyncEventArgs send = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(send);

                send.UserToken = token;
                send.Completed += socket.SendAsync;
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

            token.Init(e.AcceptSocket, receiveArgs, sendArgs);

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

                if (socket.ReConnectClient() == false)
                {
                    logger?.Error("Not Found Remote Connect Info");
                    return;
                }

                OnConnect?.Invoke(socket.SocketState, null);

                return;
            }

            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += socket.ReceiveAsync;
            byte[] receiveBuffer = new byte[65535];
            receiveArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);

            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += socket.SendAsync;
            byte[] sendBuffer = new byte[65535];
            receiveArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);

            netToken = new NetToken();
            receiveArgs.UserToken = netToken;
            sendArgs.UserToken = netToken;

            netToken.Init(socket.Socket, receiveArgs, sendArgs);
            netToken.Receive();

            OnConnect?.Invoke(socket.SocketState, netToken);
        }

        public void ReceiveComplete(SocketAsyncEventArgs e)
        {
            var data = e.UserToken as NetToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (OnReceive == null)
                {
                    logger?.Error("error not found OnReceive Func");                    
                    return;
                }

                var outputStream = data.ReceiveStream();

                if (outputStream != null)
                {
                    OnReceive?.Invoke(data.GetUserToken(), outputStream.GetBuffer());
                }

                data.Receive();
            }
            else
            {
                OnDisConnect?.Invoke(data);
            }
        }

        public void SendComplete(SocketAsyncEventArgs e)
        {
            var data = e.UserToken as NetToken;
            data.SendDequeue();
        }

        #endregion

        //클라이언트 -> 서버 패킷 전송 함수
        public void SendPacket(byte[] _sendBuffer)
        {
            netToken.SendPacket(_sendBuffer);
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
            if (socket == null)
            {
                return SocketError.SocketError;
            }

            return socket.SocketState;
        }
    }
}
