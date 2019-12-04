using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace EUDP
{
    /// <summary>
    /// Specifies general view on network. Contains all the created <see cref="T:EUDP.Connection"/> objects.
    /// Distributes messages between connections and operates on those connections.
    /// </summary>
    public class NetworkTopology
    {
        class ReceiveState{
            public byte[] buffer;
            public EndPoint endpoint;

            public ReceiveState(int bufferSize){
                buffer = new byte[bufferSize];
                endpoint = new IPEndPoint(IPAddress.Any, 0);
            }
        }

        public delegate Connection ConnectionFactory(EndPoint endpoint);
        public delegate void NewConnectionHandler(Connection connection);
        public delegate void DisconnectionHandler(Connection connection);
        public delegate void MessageHandler(Connection connection, byte[] message, int startIndex, int count);

        Socket socket;
        readonly int bufferSize;

        Dictionary<EndPoint, Connection> connections;
        ConnectionFactory connectionFactory;
        event NewConnectionHandler OnNewConnection;
        event DisconnectionHandler OnDisconnection;
        event MessageHandler OnMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EUDP.NetworkTopology"/> class and binds it to local endpoint.
        /// </summary>
        /// <param name="localEP">Local endpoint for socket binding.</param>
        public NetworkTopology(EndPoint localEP, int bufferSize = 2048){
            socket = new Socket(localEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEP);

            OnNewConnection = (connection) => { };
            OnDisconnection = (connection) => { };
            OnMessage = (Connection connection, byte[] message, int startIndex, int count) => { };
            connections = new Dictionary<EndPoint, Connection>();

            this.bufferSize = bufferSize;
        }

        /// <summary>
        /// Creates new connection to remote endpoint.
        /// </summary>
        /// <param name="remoteEP">Remote endpoint.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Connection MakeConnection(EndPoint remoteEP) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start listen for incomming messages and new connections.
        /// </summary>
        public void StartListen(){
            var state = new ReceiveState(bufferSize);
            socket.BeginReceiveFrom(state.buffer, 0, bufferSize, SocketFlags.None, ref state.endpoint, OnSocketReceiveMessage, state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnSocketReceiveMessage(IAsyncResult asyncresult) {
            var state = (ReceiveState)asyncresult.AsyncState;

            int readed = socket.EndReceiveFrom(asyncresult, ref state.endpoint);
            if(readed >= 2){
                var sender = state.endpoint;

                Connection connection;
                if(!connections.TryGetValue(sender, out connection)){
                    connection = MakeConnection(sender);
                }

                connection.ReceiveMessage(state.buffer[0], new ArraySegment<byte>(state.buffer, 2, readed - 2));
            }
            
            state.endpoint = new IPEndPoint(IPAddress.Any, 0);
            socket.BeginReceiveFrom(state.buffer, 0, bufferSize, SocketFlags.None, ref state.endpoint, OnSocketReceiveMessage, state);
        }

        /// <summary>
        /// Sends the message to the socket. If something will go wrong, responsibleConnection will be notified.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="responsibleConnection">Responsible connection.</param>
        public void SendMessage(ArraySegment<byte> data, Connection responsibleConnection){
            socket.BeginSendTo(data.Array, data.Offset, data.Count, SocketFlags.None, responsibleConnection.RemoteEndPoint, EndSend, responsibleConnection);
        }

        private void EndSend(IAsyncResult asyncResult){
            socket.EndSendTo(asyncResult);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetConnectionFactory(ConnectionFactory factory) => connectionFactory = factory;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddNewConnectionHandler(NewConnectionHandler handler) => OnNewConnection += handler;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveNewConnectionHandler(NewConnectionHandler handler) => OnNewConnection -= handler;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDisconnectionHandler(DisconnectionHandler handler) => OnDisconnection += handler;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveDisconnectionHandler(DisconnectionHandler handler) => OnDisconnection -= handler;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessageHandler(MessageHandler handler) => OnMessage += handler;
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveMessageHandler(MessageHandler handler) => OnMessage -= handler;



        ~NetworkTopology(){
            Dispose();
        }

        /// <summary>
        /// Free all the resources.
        /// </summary>
        public void Dispose(){
            socket.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
