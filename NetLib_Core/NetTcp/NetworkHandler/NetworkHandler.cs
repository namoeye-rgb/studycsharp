using NetLib.Token;
using System.Net.Sockets;
using System.Threading;

namespace NetLib
{
    public enum NETCLIENT_TYPE
    {
        CS = 1,
        S2S = 1,
    }

    public partial class NetworkHandler
    {
        private INetLogger logger;

        public NETCLIENT_TYPE NetClientType;

        //Common
        private AsyncSocket netWork;

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

        public NetworkHandler(INetLogger _logger, NETCLIENT_TYPE _type)
        {
            logger = _logger;
            netWork = new AsyncSocket();
            NetClientType = _type;
        }

        public NetworkHandler(NETCLIENT_TYPE _type)
        {
            netWork = new AsyncSocket();
            NetClientType = _type;
        }

        public void Start_Server(int _port, int _maxConnection)
        {
            //서버에만 풀을 생성해두기 위한것
            InitializePool(_maxConnection);

            netWork.AcceptClient = AddClient;
            netWork.ReceiveComplete = ReceiveComplete;
            netWork.SendComplete = SendComplete;
            netWork.StartServer(_port, _maxConnection);
        }

        public void Init_Client()
        {
            netWork.ConnectServer = ConnectServer;
            netWork.ReceiveComplete = ReceiveComplete;
            netWork.SendComplete = SendComplete;
        }
        public void Start_Client(string _ip, int _port, bool _isRecoonnect)
        {
            netWork.StartClient(_ip, _port, _isRecoonnect);
        }
        private void InitializePool(int _maxConnection)
        {
            bufferMgr = new BufferManager(_maxConnection * bufferSize * 2, bufferSize);

            receiveEventPool = new SocketAsyncEventArgsPool(50);
            sendEventPool = new SocketAsyncEventArgsPool(50);

            for (int i = 0; i < _maxConnection; ++i) {

                //recive Pool
                SocketAsyncEventArgs receive = new SocketAsyncEventArgs();
                NetToken token = new NetToken();

                bufferMgr.SetBuffer(receive);

                receive.UserToken = token;
                receive.Completed += netWork.ReceiveAsync;
                receiveEventPool.PushPool(receive);

                //send Pool
                SocketAsyncEventArgs send = new SocketAsyncEventArgs();
                bufferMgr.SetBuffer(send);

                send.UserToken = token;
                send.Completed += netWork.SendAsync;
                sendEventPool.PushPool(send);
            }
        }

        #region 소켓내부에서 Complete되고난 뒤에 처리되는 함수
        //서버용
        public void AddClient(SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs receiveArgs = receiveEventPool.PopPool();
            SocketAsyncEventArgs sendArgs = sendEventPool.PopPool();

            //toekn에 저장해두기
            NetToken token = receiveArgs.UserToken as NetToken;

            //clientList.Add(token);
            token.Init(e.AcceptSocket, receiveArgs, sendArgs, NetClientType);

            OnAccept?.Invoke(token);

            //해당 서버 <-> 클라이언트에 연결된 소켓을 리시브해서 대기한다는것임.
            //local에는 서버 remote에는 클라의 주소가 설정된다.
            //Accept을 하면서 remote의 주소가 할당된더고
            //여기서 token을 모으는 이유는 서버에서 클라로 연결을 위함이다.

            token.Ready_Receive();

            logger?.Debug("Client Accept : {0}", e.AcceptSocket.RemoteEndPoint);
        }

        //클라이언트 용
        public void ConnectServer(SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) {
                logger?.Warn("Fail Connect Server, SocketError : {0}, Remote EndPoint : {1}", e.SocketError, e.RemoteEndPoint);
                Thread.Sleep(10000);

                if (netWork.ReConnectClient() == false) {
                    logger?.Error("Not Found Remote Connect Info");
                    return;
                }

                OnConnect?.Invoke(netWork.SocketState, null);

                return;
            }

            SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += netWork.ReceiveAsync;
            byte[] receiveBuffer = new byte[65535];
            receiveArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);

            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += netWork.SendAsync;
            byte[] sendBuffer = new byte[65535];
            receiveArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);

            netToken = new NetToken();
            receiveArgs.UserToken = netToken;
            sendArgs.UserToken = netToken;

            netToken.Init(netWork.Socket, receiveArgs, sendArgs, NetClientType);
            netToken.Ready_Receive();

            OnConnect?.Invoke(netWork.SocketState, netToken);
        }

        public void ReceiveComplete(SocketAsyncEventArgs e)
        {
            var data = e.UserToken as NetToken;

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) {
                if (OnReceive == null) {
                    return;
                }

                var outputStream = data.ReceivePacket();

                if (outputStream != null) {
                    OnReceive?.Invoke(data.GetUserToken(), outputStream.GetBuffer());
                }

                data.Ready_Receive();
            } else {
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
            if (netWork == null) {
                return SocketError.SocketError;
            }

            return netWork.SocketState;
        }
    }
}
