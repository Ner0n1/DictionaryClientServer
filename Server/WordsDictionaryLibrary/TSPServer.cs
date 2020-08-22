using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ConnectionProcessing;

namespace TSPUsage

{
    public class TSPServer
    {
        static TcpListener listener;

        public TcpListener StartListener(string port)
        {

            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), int.Parse(port));
            listener.Start();
            return listener;
        }       
    }
}

