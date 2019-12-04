using System;
using System.Net;
using EUDP.Chanels;
using EUDP.Chanels.Nodes;
using EUDP.Chanels.Nodes.Communication;
using EUDP.Chanels.Nodes.Network;

namespace EUDP
{
    class MainClass
    {
        public static void Main(string[] args){
            Console.WriteLine("Hello World!");

            NetworkTopology topology = new NetworkTopology(new IPEndPoint(IPAddress.Any, 55333));
            topology.SetConnectionFactory((endpoint) => {
                var con = new Connection(endpoint);
                
                //Chanels
                var rawChanel = new Chanel<byte[]>();
                rawChanel.AddNode(new EventCommunication<byte[]>());
                rawChanel.AddNode(new Raw(topology, con));
                rawChanel.FinishInit();
                con.AddChanel(rawChanel);

                return con;
            });

            topology.AddNewConnectionHandler((c) => { Console.WriteLine("New client connected"); });
            topology.AddDisconnectionHandler((c) => { Console.WriteLine("Client disconnected"); });

            topology.StartListen();


            Console.WriteLine("Hello World!");
        }
    }
}
