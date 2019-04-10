using LoadBalencerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadBalencerAlgorithmsCL
{
    public class First : IAlgorithm
    {
        public string Name
        {
            get => this.GetType().Name;
        }

        public string ServerPort { get; set; }

        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            return allServers.Count > 0 ?  allServers.First() : null;
        }
    }
}
