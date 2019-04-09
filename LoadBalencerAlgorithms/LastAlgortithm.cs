using LoadBalencerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerAlgorithms
{
    public class LastAlgorithm : IAlgorithm
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

        public Server GetBestServer(List<Server> allServers)
        {
            return allServers.Last();
        }
        string IAlgorithm.GetName()
        {
            return this.name;
        }
    }
}
