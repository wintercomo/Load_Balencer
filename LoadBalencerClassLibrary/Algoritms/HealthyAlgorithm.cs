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
            get => this.GetType().Name;
            set
            {
                this.name = value;
            }
        }
        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            List<Server> onlineServers = allServers.Where(server => server.Status == "Normal").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Busy").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Down").ToList();
            return onlineServers.Count > 0 ?  onlineServers.First() : null;
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
