using System;
using System.Runtime.CompilerServices;

namespace EUDP.Chanels.Nodes.Communication{
    /// <summary>
    /// Used as input node to the chanel pipe from client (local) side. 
    /// Forwards local messages without modifications to specified <see cref="INodeReceiver{T}"/>
    /// For every message received from remote raises event immediately.
    /// Hadlers for this event can be added using <see cref="EventCommunication{T}.AddMessageHandler(EventCommunication{T}.MessageHandler)"/>
    /// There is no guaranty on which thread event will be raised, but there will not be two events raised at same time.
    /// </summary>
    public class EventCommunication<T>: INode<T,T>{
        public delegate void MessageHandler(T message);

        event MessageHandler OnMessageReceived;
        private INodeReceiver<T> receiver;

        public EventCommunication(){
            OnMessageReceived = (x) => { };
        }

        /// <summary>
        /// Adds handler that will be executed when new message will arrive from remote client
        /// </summary>
        /// <param name="handler">Handler.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessageHandler(MessageHandler handler){
            OnMessageReceived += handler;
        }

        /// <summary>
        /// Removes the message handler.
        /// </summary>
        /// <param name="handler">Handler.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveMessageHandler(MessageHandler handler){
            OnMessageReceived -= handler;
        }

        void INodeReceiver<T>.SetSender(INodeSender<T> sender){}

        void INodeReceiver<T>.Receive(T data){
            receiver.Receive(data);
        }

        void INodeSender<T>.SetReceiver(INodeReceiver<T> receiver){
            this.receiver = receiver;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void INodeSender<T>.Receive(T data){
            OnMessageReceived(data);
        }
    }
}
