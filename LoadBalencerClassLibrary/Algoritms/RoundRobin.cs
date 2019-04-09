using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class RoundRobin : IAlgorithm
    {
        int index = 0;

        public Server GetBestServer(List<Server> allServers, int serverPort = 0)
        {
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
