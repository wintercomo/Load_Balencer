using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary.Algoritms
{
    class RandomAlgorithm : IAlgorithm
    {

        public string Name
        {
            get => this.GetType().Name;
        }

        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            var random = new Random();
            int index = random.Next(allServers.Count);
            return allServers[index];
        }

        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
