﻿using LoadBalencerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoadBalencerAlgorithms
{
    public class LastAlgorithm : IAlgorithm
    {
       
        public string Name
        {
            get => this.GetType().Name;
        }

        public Server GetBestServer(List<Server> allServers, string[] cookieParams = null)
        {
            return allServers.Last();
        }
    }
}
