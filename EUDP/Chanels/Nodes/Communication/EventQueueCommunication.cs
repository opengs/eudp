using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EUDP.Chanels.Nodes.Communication{
    /// <summary>
    /// Used as input node to the chanel pipe from client (local) side. 
    /// Forwards local messages without modifications to specified <see cref="INodeReceiver{T}"/>
    /// Every message received from remote will be added to the queue.
    /// After <see cref="EventQueueCommunication{T}.Operate"/>, assigned handlers will be executed for every message in the queue.
    /// Hadlers for this event can be added using <see cref="EventQueueCommunication{T}.AddMessageHandler(EventCommunication{T}.MessageHandler)"/>
    /// There is no guaranty on which thread event will be raised, but there will not be two events raised at same time.
    /// </summary>
    public class EventQueueCommunication<T> : INode<T, T>, IOperateAble, ICloseAble {
        Queue<T> messages;
        EventCommunication<T> eventCommunication;

        public EventQueueCommunication() {
            eventCommunication = new EventCommunication<T>();
            messages = new Queue<T>();
        }

        /// <summary>
        /// Raise event for every message in queue
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Operate() {
            while (messages.Count > 0)
                ((INodeSender<T>)eventCommunication).Receive(messages.Dequeue());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void INodeReceiver<T>.Receive(T data) => ((INodeReceiver<T>)eventCommunication).Receive(data);
        [MethodImpl(MethodImplOptions.Synchronized)]
        void INodeSender<T>.Receive(T data) => messages.Enqueue(data);

        [MethodImpl(MethodImplOptions.Synchronized)]
        void INodeSender<T>.SetReceiver(INodeReceiver<T> receiver) => ((INodeSender<T>)eventCommunication).SetReceiver(receiver);
        [MethodImpl(MethodImplOptions.Synchronized)]
        void INodeReceiver<T>.SetSender(INodeSender<T> sender) => ((INodeReceiver<T>)eventCommunication).SetSender(sender);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddMessageHandler(EventCommunication<T>.MessageHandler handler) => eventCommunication.AddMessageHandler(handler);
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveMessageHandler(EventCommunication<T>.MessageHandler handler) => eventCommunication.RemoveMessageHandler(handler);

        public void Close() => Operate();
    }
}
