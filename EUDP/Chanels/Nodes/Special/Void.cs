using System;
namespace EUDP.Chanels.Nodes.Special{
    /// <summary>
    /// Voids all the received data. 
    /// Doesnt path data forward or backward. Does nothing with data.
    /// </summary>
    public class Void<R, S> : INode<R, S>{
        public void Receive(R data){}
        public void Receive(S data){}
        public void SetReceiver(INodeReceiver<S> receiver){}
        public void SetSender(INodeSender<R> sender){}
    }
}
