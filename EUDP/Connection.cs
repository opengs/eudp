using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using EUDP.Chanels;

namespace EUDP
{
    /// <summary>
    /// Represents connection between two endpoints.
    /// </summary>
    public class Connection {
        public EndPoint RemoteEndPoint { get; }
        protected List<object> chanels;

        public Connection(EndPoint endpoint) {
            RemoteEndPoint = endpoint;
            chanels = new List<object>();
        }

        /// <summary>
        /// Adds the chanel to the connection.
        /// Added chanels order is important, because every chanel receives id based on this order.
        /// Chanel order should be the same on all endpoints. 
        /// </summary>
        /// <param name="chanel">Chanel to add.</param>
        public void AddChanel<T>(Chanel<T> chanel){
            chanels.Add(chanel);
        }

        /// <summary>
        /// Receive message on this connection.
        /// </summary>
        /// <param name="data">Received data.</param>
        public void ReceiveMessage(int chanel, ArraySegment<byte> data){
            //chanels[chanel].ReceiveMessage(data);
        }

        /// <summary>
        /// Sends data to the specified chanel of the connection.
        /// </summary>
        /// <param name="chanel">Chanel id to send.</param>
        /// <param name="data">Data buffer.</param>
        public void SendData<T>(int chanel, T data){
            SendData((Chanel<T>)chanels[chanel], data);
        }

        /// <summary>
        /// Sends data to the specified chanel of the connection.
        /// </summary>
        /// <param name="chanel">Chanel to send.</param>
        /// <param name="data">Data buffer.</param>
        public void SendData<T>(Chanel<T> chanel, T data){
            throw new NotImplementedException();
        }
    }
}
