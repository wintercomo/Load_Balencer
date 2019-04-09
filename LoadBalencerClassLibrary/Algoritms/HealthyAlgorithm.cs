using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class HealthyAlgorithm : IAlgorithm
    {
        string name;
        public string Name
        {
            get => name;
            set
            {
                this.name = value;
            }
        }

        public string ServerPort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Server GetBestServer(List<Server> allServers, int serverPort = 0)
        {
            List<Server> onlineServers = allServers.Where(server => server.Status == "Normal").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Busy").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Down").ToList();
            return allServers.First();
        }

        public string GetName()
        {
            return this.name;
        }
    }
}
