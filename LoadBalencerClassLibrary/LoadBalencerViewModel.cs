using System;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerViewModel : BindableBase
    {
        int port = 8080;

        public int Port { get => port;
            set
            {
                if (SetProperty<int>(ref port, value)) this.port = value;
            }
        }
    }
}
