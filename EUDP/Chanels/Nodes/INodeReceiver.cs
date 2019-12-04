using System;
namespace EUDP.Chanels.Nodes{
    /// <summary>
    /// Specifies, that object can receive messages from <see cref="INodeSender{T}"/>
    /// </summary>
    public interface INodeReceiver<T>{
        void SetSender(INodeSender<T> sender);
        void Receive(T data);
    }
}
