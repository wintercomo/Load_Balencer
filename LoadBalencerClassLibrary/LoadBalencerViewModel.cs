using System;
using System.Collections.Generic;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerViewModel : BindableBase
    {
        int port = 8080;
        List<Algorithm> algorithms = new List<Algorithm> { { new Algorithm() { Name = "Option 1"} } };

        public int Port
        {
            get => port;
            set
            {
                if (SetProperty(ref port, value)) this.port = value;
            }
        }

        public List<Algorithm> Algorithms
        {
            get => algorithms;
            set
            {
                if (SetProperty(ref algorithms, value)) this.algorithms = value;
            }
        }
    }
}
