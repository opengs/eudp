using System;
using System.Net;
using System.Net.Sockets;

namespace EUDP.Chanels.Nodes.Network{

    /// <summary>
    /// Outputs to and receives from topology raw byte data. 
    /// Assotiates itself with specific connection.
    /// </summary>
    public class Raw: INode<ArraySegment<byte>, ArraySegment<byte>>{
        readonly NetworkTopology topology;
        readonly Connection connection;

        INodeSender<ArraySegment<byte>> sender;

        public Raw(NetworkTopology topology, Connection connection){
            this.topology = topology;
            this.connection = connection;
        }

        void INodeReceiver<ArraySegment<byte>>.Receive(ArraySegment<byte> data){
            topology.SendMessage(data, connection);
        }

        void INodeSender<ArraySegment<byte>>.Receive(ArraySegment<byte> data){
            sender.Receive(data);
        }

        void INodeSender<ArraySegment<byte>>.SetReceiver(INodeReceiver<ArraySegment<byte>> receiver) { }
        void INodeReceiver<ArraySegment<byte>>.SetSender(INodeSender<ArraySegment<byte>> sender) => this.sender = sender;
    }
}
