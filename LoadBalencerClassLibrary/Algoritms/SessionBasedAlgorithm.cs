using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class SessionBasedAlgorithm : IAlgorithm
    {
        public string Name
        {
            get => this.GetType().Name;
        }
        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            if (cookieParams == null) return allServers.First(server => server.Status != "Not running");
            string sessionId = cookieParams[1].Split('=')[1]; // cookieParam[1] = always sessionId
            Console.WriteLine($"Session ID {sessionId} To server");
            foreach (var server in allServers) foreach (var session in server.Sessions) if (session.SessionId.ToString() == sessionId) return server;
            return allServers.First(server => server.Status != "Not running");
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
