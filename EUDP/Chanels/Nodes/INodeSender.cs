using System;
namespace EUDP.Chanels.Nodes{
    /// <summary>
    /// Specifies, that object can send messages to <see cref="INodeReceiver{T}"/>
    /// </summary>
    public interface INodeSender<T>{
        void SetReceiver(INodeReceiver<T> receiver);
        void Receive(T data);
    }
}
