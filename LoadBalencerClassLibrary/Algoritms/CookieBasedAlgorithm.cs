using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class CookieBasedAlgorithm : IAlgorithm
    {
        string serverPort;

        public Server GetBestServer(List<Server> allServers, int serverPort = 0)
        {
            {
                Server wantedServer = null;
                if (serverPort == 0) return wantedServer;
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
