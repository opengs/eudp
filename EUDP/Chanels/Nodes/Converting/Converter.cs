﻿using System;
using System.Net.Sockets;

namespace EUDP.Chanels.Nodes.Converting{
    public abstract class Converter<R,S> : INode<R, S>{
        INodeReceiver<S> receiver;
        INodeSender<R> sender;

        protected abstract S ConvertForward(R data);
        protected abstract R ConvertBackward(S data);

        void INodeReceiver<R>.Receive(R data){
            receiver.Receive(ConvertForward(data));
        }

        void INodeSender<S>.Receive(S data){
            sender.Receive(ConvertBackward(data));
        }

        void INodeSender<S>.SetReceiver(INodeReceiver<S> receiver) => this.receiver = receiver;
        void INodeReceiver<R>.SetSender(INodeSender<R> sender) => this.sender = sender;
    }
}
