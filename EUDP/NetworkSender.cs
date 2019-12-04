using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EUDP
{
    public class NetworkSender{
        struct SendData{
            public int offset;
            public int size;
            public byte[] buffer;

            public SendData(int offset, int size, byte[] buffer){
                this.offset = offset;
                this.size = size;
                this.buffer = buffer;
            }
        }

        object sendLocker = new object();
        SemaphoreSlim sem;
        bool sendInProgress;
        ConcurrentQueue<SendData> sendQueue = new ConcurrentQueue<SendData>();
        Socket socket;

        public NetworkSender(Socket socket){}


        public void Send(byte[] data, int offset, int size, Connection connection){
            sendQueue.Enqueue(new SendData(offset, size, data));
            TryBeginSending();
        }

        void OnSended(IAsyncResult sendResult){
            socket.EndSend(sendResult);
        }

        void TryBeginSending(){
            if (sendInProgress) return;
            lock (sendLocker){
                if (sendInProgress) return;
                sendInProgress = TrySendMessageFromQueue();
            }
        }

        bool TrySendMessageFromQueue(){
            return false;
        }
    }
}
