using System;
using System.Collections.Generic;
using EUDP.Chanels.Nodes;
namespace EUDP.Chanels{
    /// <summary>
    /// Represents communication chanel inside connection. Chanels are separated from each other, and handles messages asynchronously from each other.
    /// Each chanel has nodes pipeline (nodes connected with each other) that processes and sends message.
    /// </summary>
    public class Chanel<T>{
        List<object> nodes = new List<object>();

        INodeReceiver<T> inputNode;
        INodeSender<ArraySegment<byte>> outputNode;

        List<ICloseAble> closeAbleNodes = new List<ICloseAble>();
        List<IOperateAble> operateAbleNodes = new List<IOperateAble>();


        /// <summary>
        /// Adds the node to the nodes pipeline.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <typeparam name="R">Node receiver type.</typeparam>
        /// <typeparam name="S">Node sender type.</typeparam>
        public void AddNode<R,S>(INode<R,S> node){
            nodes.Add(node);
            if (node is ICloseAble) closeAbleNodes.Add(node as ICloseAble);
            if (node is IOperateAble) operateAbleNodes.Add(node as IOperateAble);
            if (nodes.Count == 1) return;

            var sender = (INodeSender<R>)nodes[nodes.Count - 2];
            sender.SetReceiver(node);
            node.SetSender(sender);
        }

        /// <summary>
        /// Should be executed after chanel creation and nodes adding. 
        /// </summary>
        public void FinishInit(){
            inputNode = (INodeReceiver<T>)nodes[0];
            outputNode = (INodeSender<ArraySegment<byte>>)nodes[nodes.Count - 1];
        }

        /// <summary>
        /// Sends message to first node in nodes pipeline.
        /// </summary>
        /// <param name="message">Message.</param>
        public void SendMessage(T message){
            inputNode.Receive(message);
        }

        /// <summary>
        /// Sends message to last node in nodes pipeline indicating, that message was received from connection.
        /// </summary>
        /// <param name="message">Message.</param>
        public void ReceiveMessage(ArraySegment<byte> message){
            outputNode.Receive(message);
        }
    }
}
