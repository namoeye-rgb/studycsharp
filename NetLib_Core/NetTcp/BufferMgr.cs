using System.Collections.Generic;
using System.Net.Sockets;

namespace NetLib
{
    public class BufferManager
    {
        private int numBytes;
        private byte[] buffer;
        private Stack<int> freeIndexPool;
        private int currentIndex;
        private int bufferSize;

        public BufferManager(int totalBytes, int size)
        {
            numBytes = totalBytes;
            currentIndex = 0;
            bufferSize = size;
            freeIndexPool = new Stack<int>();

            InitBuffer();
        }

        public void InitBuffer()
        {
            //하나의 큰 걸 나누어서 각각 SocketAsyncEventArg의 Object에서 나눠서 사용한다
            buffer = new byte[numBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (freeIndexPool.Count > 0)
            {
                args.SetBuffer(buffer, freeIndexPool.Pop(), bufferSize);
            }
            else
            {
                if ((numBytes - bufferSize) < currentIndex)
                {
                    return false;
                }

                args.SetBuffer(buffer, currentIndex, bufferSize);
                currentIndex += bufferSize;
            }

            return true;
        }

        //사용하지 않은 버퍼를 반환시키기 위한 일을 함.
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
