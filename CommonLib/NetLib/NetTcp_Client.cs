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
    public class NetTcp_Client
    {
        ILogger logger = null;

        public delegate void AsyncReadCallback(MemoryStream _stream);
        public delegate void AsyncConnectCallback(bool _ret);
        public delegate void AsyncDisconnectCallback();

        public AsyncReadCallback OnRead;
        public AsyncDisconnectCallback OnDisconnect;
        

        Socket tcpSocket;

        public static int SendBufferSize = 65535;
        public static int RecvBufferSize = 65535;

        int offset = 0;

        byte[] sendBuffer = null;
        private byte[] recvstream = new byte[RecvBufferSize];

        public void SetSocket(Socket _socket)
        {
            tcpSocket = _socket;
        }

        public bool Connect(string ip, ushort port)
        {
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
            if (tcpSocket == null) { return true; }
            return false;
        }

        public void Disconnect()
        {
            if (tcpSocket == null) { 
                return; 
            }

            try
            {
                tcpSocket.Close(0);
                OnDisconnect?.Invoke();
            }
            catch
            {

            }
            finally
            {
                tcpSocket = null;
            }
        }

        public void Write(MemoryStream _stream)
        {
            if (_stream.Length == 0)
            {
                return;
            }

            sendBuffer = _stream.ToArray();

            tcpSocket.BeginSend(sendBuffer, 0, (int)sendBuffer.Length, SocketFlags.None, SendComplete, null);
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

        private void Defragment(MemoryStream _transferred)
        {
            var buffer = _transferred.GetBuffer();

            int blockSize = 0;
            int readBytes = 0;

            while (_transferred.Length - readBytes > sizeof(int))
            {
                blockSize = BitConverter.ToInt32(buffer, readBytes);
                if (blockSize < 1 || blockSize > 65535)
                {
                    Console.WriteLine($"{this.GetType()} -> BlockSize Wrong {blockSize} Disconnect");
                    _transferred.Seek(_transferred.Length, SeekOrigin.Begin);
                    Disconnect();
                    return;
                }

                if (blockSize + readBytes > _transferred.Length) { break; }

                Stream stream = new MemoryStream(buffer, readBytes + 4, blockSize - 4, true, true);
                readBytes += blockSize;

                //CryptoStream csEncrypt = null;
                //if (aesAlg != null)
                //{
                //    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                //    csEncrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
                //    stream = csEncrypt;
                //}
                //GZipStream compressionStream = null;
                //if (UseCompress)
                //{
                //    compressionStream = new GZipStream(stream, CompressionMode.Decompress, true);
                //    stream = compressionStream;
                //}

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
                int len = (int)tcpSocket.EndReceive(ar, out error);
                if (len == 0)
                {
                    Disconnect();
                    return;
                }

                len = offset + len;

                MemoryStream transferred = new MemoryStream(recvstream, 0, len, true, true);
                Defragment(transferred);
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
                if (tcpSocket == null) { 
                    return; 
                }
                var len = tcpSocket.EndSend(ar);
                if (len > 0)
                {
                    sendBuffer = null;
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

    }
}
