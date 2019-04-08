using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary
{
    public class Server : BindableBase
    {
        private string serverURL;
        private int port;
        private string status;

        public Server(string serverURL, int port, string status)
        {
            this.ServerURL = serverURL;
            this.Port = port;
            this.Status = status;
        }

        public string ServerURL
        {
            get => serverURL;
            set
            {
                if (SetProperty(ref serverURL, value)) this.serverURL = value;
            }
        }
        public int Port
        {
            get => port;
            set
            {
                if (SetProperty(ref port, value)) this.port = value;
            }
        }
        public string Status
        {
            get => status;
            set
            {
                if (SetProperty(ref status, value)) this.status = value;
            }
        }
    }
}
