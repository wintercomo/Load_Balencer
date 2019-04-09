using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class CookieBasedAlgorithm : IAlgorithm
    {
        string serverPort;

        public string Name
        {
            get => this.GetType().Name;
        }

        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            {
                Server wantedServer = null;
                if (cookieParams == null) return wantedServer;
                int serverPort = int.Parse(cookieParams[0].Split('=')[1]);
                wantedServer = allServers.First(server => server.Port == serverPort);
                if (wantedServer == null) wantedServer = allServers.First(server => server.Status != "Not Running");
                return wantedServer;
            }
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }
}
