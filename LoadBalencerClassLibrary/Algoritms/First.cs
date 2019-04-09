using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary
{
    public class First : IAlgorithm
    {
        string name;
        string value;

        public string Name
        {
            get => name;
            set
            {
                this.name = value;
            }
        }

        public string ServerPort { get; set; }

        public Server GetBestServer(List<Server> allServers, int serverPort = 0)
        {
            return allServers.First();
        }
    }
}
