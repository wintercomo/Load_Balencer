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
        Boolean selected = false;

        public string Name
        {
            get => name;
            set
            {
                this.name = value;
            }
        }

        public Server GetBestServer(List<Server> allServers)
        {
            return allServers.First();
        }
        string IAlgorithm.Name()
        {
            return this.name;
        }
    }
}
