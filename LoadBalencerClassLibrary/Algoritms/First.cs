using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary
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
            return allServers.First();
        }
    }
}
