using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class RoundRobin : IAlgorithm
    {
        int index = 0;
        public string Name
        {
            get => this.GetType().Name;
        }
        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            if (allServers.Count == 0) return null;
            if (index >= allServers.Count) index = 0;
            int serverIndex = index;
            index++;
            return allServers[serverIndex];
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }
}
