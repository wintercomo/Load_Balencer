using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class RandomAlgorithm : IAlgorithm
    {

        public Server GetBestServer(List<Server> allServers, int serverPort = 0)
        {
            var random = new Random();
            int index = random.Next(allServers.Count);
            return allServers[index];
        }
    }
}
