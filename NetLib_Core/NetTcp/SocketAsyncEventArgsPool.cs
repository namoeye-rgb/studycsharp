using System.Collections.Generic;
using System.Net.Sockets;

namespace NetLib
{
    public class SocketAsyncEventArgsPool
    {
        private Stack<SocketAsyncEventArgs> eventList;

        public SocketAsyncEventArgsPool(int size)
        {
            eventList = new Stack<SocketAsyncEventArgs>(size);
        }

        public SocketAsyncEventArgs PopPool()
        {
            lock (eventList) {
                if (eventList.Count > 0) {
                    return eventList.Pop();
                } else {
                    return null;
                }
            }
        }

        public void PushPool(SocketAsyncEventArgs socketEvent)
        {
            if (socketEvent == null) {
                return;
            }

            lock (eventList) {
                eventList.Push(socketEvent);
            }
        }
    }
}
