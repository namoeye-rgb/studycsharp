using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System.Collections;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Net.Sockets;
using UtilLib.Log;

namespace NetLib
{
    public class NetTcp_Server
    {
        ILogger logger;

        protected Aes aesAlg = null;
        protected bool UseCompress { get; set; } = false;
        public delegate void AsyncReadCallback(MemoryStream _stream);
        public delegate void AsyncConnectCallback(bool _ret);
        public delegate void AsyncAcceptCallback(Socket _acceptSocket);
        public delegate void AsyncDisconnectCallback();

        public AsyncReadCallback OnRead;
        public AsyncAcceptCallback OnAccept;
        public AsyncDisconnectCallback OnDisconnect;

        Socket tcpSocket;

        protected string ip { get; set; }
        protected ushort port { get; set; }

        public static int SendBufferSize = 65535;
        public static int RecvBufferSize = 65535;

        int offset = 0;

        byte[] sendBuffer = null;
        private byte[] recvstream = new byte[RecvBufferSize];

        enum State
        {
            IDLE = 0,
            WAIT,
            RUN,
            CLOSING,
            CLOSE,
        }

        public void SetSocket(Socket _socket)
        {
            tcpSocket = _socket;
        }

        public void Init_Server(int _port, int _maxConnection, ILogger _logger)
        {
            logger = _logger;

            tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);

            tcpSocket.Bind(ep);
            tcpSocket.Listen(_maxConnection);
        }

        public bool Accept()
        {
            if (tcpSocket != null)
            {
                return false;
            }
            try
            {
                tcpSocket.BeginAccept(new AsyncCallback(AcceptComplete), tcpSocket);
                return true;
            }
            catch (Exception e)
            {
                logger.Error($"Fail Accept Listen {e}");
            }
            return false;
        }

        public bool Connect(string ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
            if (tcpSocket != null) return false;

            try
            {
                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpSocket.BeginConnect(ip, port, ConnectComplete, null);
                return true;
            }
            catch (Exception e)
            {
                logger.Error($"Fail Connect {e}");
                Disconnect();
            }
            return false;
        }

        public bool IsClosed()
        {
            lock (this)
            {
                if (tcpSocket == null) { return true; }
                return false;
            }
        }

        public void Disconnect()
        {

            lock (this)
            {
                if (tcpSocket == null) { return; }


                try
                {
                    tcpSocket.Close(0);
                    pendings.Clear();
                }
                catch
                {

                }
                finally
                {
                    tcpSocket = null;
                }
            }

            try
            {
                OnDisconnect?.Invoke();
            }
            catch
            {

            }
        }

        private Queue<object> pendings = new Queue<object>();
        //public bool Write(Engine.Framework.ISerializable msg)
        //{
        //    if (socket == null) return false;
        //    try
        //    {
        //        lock (this)
        //        {
        //            pendings.Enqueue(msg);
        //            if (sendBuffer != null) { return true; }
        //            flush();
        //            return true;
        //        }
        //    }
        //    catch
        //    {
        //        Disconnect();
        //        return false;
        //    }
        //}

        protected virtual void flush()
        {
            lock (pendings)
            {
                if (pendings.Count == 0) { return; }
                MemoryStream output = new MemoryStream();
                output.Write(BitConverter.GetBytes((int)0), 0, 4);
                output.Seek(4, SeekOrigin.Begin);
                CryptoStream csEncrypt = null;
                Stream stream = output;
                if (aesAlg != null)
                {
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    csEncrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
                    stream = csEncrypt;
                }

                GZipStream compressionStream = null;
                if (UseCompress)
                {
                    compressionStream = new GZipStream(stream, CompressionMode.Compress, true);
                    stream = compressionStream;
                }


                while (pendings.Count > 0)
                {
                    var msg = pendings.Dequeue();
                    switch (msg)
                    {
                        //case Engine.Framework.ISerializable serializable:
                        //    serializable.Serialize(stream);
                        //    break;
                        //case MemoryStream ms:
                        //    try
                        //    {
                        //        ms.CopyTo(stream);
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        Console.WriteLine(e);
                        //    }

                        //    break;
                        //default:
                        //    break;
                    }
                }

                if (compressionStream != null)
                {
                    compressionStream.Flush();
                    compressionStream.Dispose();
                }

                if (csEncrypt != null)
                {
                    csEncrypt.FlushFinalBlock();
                }

                if (output.Length == 2)
                {
                    return;
                }

                output.Seek(0, SeekOrigin.Begin);
                output.Write(BitConverter.GetBytes((int)output.Length), 0, 4);
                output.Seek(0, SeekOrigin.Begin);

                sendBuffer = output.ToArray();

                if (csEncrypt != null)
                {
                    csEncrypt.Dispose();
                }

                output.Dispose();

                if (sendBuffer == null || sendBuffer.Length == 0)
                {
                    sendBuffer = null;
                    return;
                }

                tcpSocket.BeginSend(sendBuffer, 0, (int)sendBuffer.Length, SocketFlags.None, SendComplete, null);
            }

        }
        public bool Write(MemoryStream stream)
        {
            if (stream.Length == 0) return true;

            try
            {
                lock (this)
                {
                    if (tcpSocket == null) return false;
                    stream.Seek(0, SeekOrigin.Begin);
                    pendings.Enqueue(stream);
                    if (sendBuffer == null) { flush(); }
                    return true;
                }
            }
            catch
            {
                Disconnect();
                return false;
            }
        }

        public void Send(MemoryStream _stream)
        {
            if (_stream.Length == 0)
            {
                return;
            }

            sendBuffer = _stream.ToArray();

            //tcpSocket.SendAsync(sendBuffer, 0, (int)sendBuffer.Length, SocketFlags.None, SendComplete, null);
        }

        private void AcceptComplete(IAsyncResult ar)
        {
            Socket listen = (Socket)ar.AsyncState;

            try
            {
                var socket = listen.EndAccept(ar);
                OnAccept(socket);
                socket.BeginReceive(recvstream, 0, RecvBufferSize, SocketFlags.None, new AsyncCallback(RecvComplete), null);

                return;
            }
            catch (Exception e)
            {
                logger.Warn($"Fail Accept Listen {e}");
                Disconnect();
            }
        }

        private void ConnectComplete(IAsyncResult ar)
        {

            //try
            //{
            //    tcpSocket.EndConnect(ar);
            //    tcpSocket.BeginReceive(retcpSocket 0, RecvBufferSize, SocketFlags.NosocketsyncCallback(RecvComplete), null);
            //    OnConnect(true);
            //    if (sendBuffer == null) { flush(); }
            //    return;
            //}
            //catch (Exception e)
            //{

            //}

            //OnConnect(false);
            //Disconnect();

        }

        private void Defragment(byte[] _buffer, int _recvByteCount)
        {
            //방금 받은 recvBuffer 65535에서 받은만큼만 _transferred 로 넘어온다
            int blockSize = 0;
            int readBytes = 0;

            //기본적인 4바이크 크기(패킷전체길이)가 조건되어야만 진행
            if (_buffer.Length < PacketLib.PacketDefine.CS_PacketSize)
            {
                return;
            }

            //4바이트 크기에 있는 전체 패킷 사이즈 확인
            blockSize = BitConverter.ToInt32(_buffer, readBytes);
            //NOTE : 이 이상 패킷에 대해서는 어떻게 처리할지..
            if (blockSize < 1 || blockSize > 65535)
            {
                Console.WriteLine($"{this.GetType()} -> BlockSize Wrong {blockSize} Disconnect");
                //_transferred.Seek(_transferred.Length, SeekOrigin.Begin);
                Disconnect();
                return;
            }

            while (_buffer.Length - readBytes > blockSize)
            {
                //4바이트 뺀 나머지 버퍼값을 stream에 넣는다
                Stream stream = new MemoryStream(buffer, readBytes + 4, blockSize - 4, true, true);
                readBytes += blockSize;

                MemoryStream result = new MemoryStream();

                stream.CopyTo(result);
                result.Seek(0, SeekOrigin.Begin);

                OnRead?.Invoke(result);
            }

            _transferred.Seek(readBytes, SeekOrigin.Begin);

        }

        private void Defragment(MemoryStream _transferred)
        {
            //방금 받은 recvBuffer 65535에서 받은만큼만 _transferred 로 넘어온다
            var buffer = _transferred.GetBuffer();

            int blockSize = 0;
            int readBytes = 0;

            //기본적인 4바이크 크기(패킷전체길이)가 조건되어야만 진행



            while (_transferred.Length - readBytes > sizeof(int))
            {
                //4바이트 크기에 있는 전체 패킷 사이즈 확인
                blockSize = BitConverter.ToInt32(buffer, readBytes);
                if (blockSize < 1 || blockSize > 65535)
                {
                    Console.WriteLine($"{this.GetType()} -> BlockSize Wrong {blockSize} Disconnect");
                    //??
                    //_transferred.Seek(_transferred.Length, SeekOrigin.Begin);
                    Disconnect();
                    return;
                }

                if (blockSize + readBytes > _transferred.Length)
                {
                    break;
                }

                //4바이트 뺀 나머지 버퍼값을 stream에 넣는다
                Stream stream = new MemoryStream(buffer, readBytes + 4, blockSize - 4, true, true);
                readBytes += blockSize;

                MemoryStream result = new MemoryStream();

                stream.CopyTo(result);
                result.Seek(0, SeekOrigin.Begin);

                OnRead?.Invoke(result);
            }

            _transferred.Seek(readBytes, SeekOrigin.Begin);

        }

        private void RecvComplete(IAsyncResult ar)
        {
            SocketError error;
            try
            {
                //len : 현재 받은 바이트 수
                int len = (int)tcpSocket.EndReceive(ar, out error);
                if (len == 0)
                {
                    Disconnect();
                    return;
                }
                
                len = offset + len;

                //받은 buffer가 recvStream에 저장되어 넘어온다
                MemoryStream transferred = new MemoryStream(recvstream, 0, len, true, true);
                Defragment(recvstream, len);
                offset = (int)len - (int)transferred.Position;
                if (offset < 0)
                {
                    Disconnect();
                    return;
                }

                Array.Copy(recvstream, transferred.Position, recvstream, 0, offset);
                tcpSocket.BeginReceive(recvstream, offset, RecvBufferSize - offset, SocketFlags.None, new AsyncCallback(RecvComplete), null);
                return;

            }
            catch (Exception e)
            {
                logger.Error($"Fail RecvComplete, {e}");
            }

            Disconnect();
        }

        private void SendComplete(IAsyncResult ar)
        {
            try
            {
                int len = 0;
                lock (this)
                {
                    if (tcpSocket == null) { return; }
                    len = tcpSocket.EndSend(ar);
                    if (len > 0)
                    {
                        sendBuffer = null;
                        flush();
                    }
                }
                if (len == 0)
                {
                    Disconnect();
                    return;
                }
            }
            catch (Exception e)
            {
                logger.Error($"Fail SendComplete, {e}");
                Disconnect();
            }
        }



        public EndPoint RemoteEndPoint {
            get {
                try
                {
                    return tcpSocket.RemoteEndPoint;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
