using System;
namespace EUDP.Chanels.Nodes{
    /// <summary>
    /// Specifies, that object is chanel node. Nodes creates chanel pipe and are used for message transformation before sending 
    /// or transformating received data back into message.
    /// </summary>
    public interface INode<R,S> : INodeReceiver<R>, INodeSender<S>{
    }
}
