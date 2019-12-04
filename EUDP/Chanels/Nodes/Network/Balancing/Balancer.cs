using System;
namespace EUDP.Chanels.Nodes.Network.Balancing{
    public abstract class Balancer<T>: INode<byte[], byte[]>{
        public Balancer(){}




        void INodeReceiver<byte[]>.Receive(byte[] data){
            throw new NotImplementedException();
        }

        void INodeSender<byte[]>.Receive(byte[] data){
            throw new NotImplementedException();
        }

        void INodeSender<byte[]>.SetReceiver(INodeReceiver<byte[]> receiver){
            throw new NotImplementedException();
        }

        void INodeReceiver<byte[]>.SetSender(INodeSender<byte[]> sender){
            throw new NotImplementedException();
        }
    }
}
