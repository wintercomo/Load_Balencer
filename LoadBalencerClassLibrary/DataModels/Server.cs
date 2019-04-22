using LoadBalencerClassLibrary.DataModels;
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
        private string displayColor;

        public Server(string serverURL, int port, string status)
        {
            this.ServerURL = serverURL;
            this.Port = port;
            this.Status = status;
            this.Sessions = new List<Session>();
        }
        public List<Session> Sessions { get; set; }

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
                switch (value)
                {
                    case "Not running":
                        DisplayColor =  "Gray";
                        break;
                    case "Normal":
                        DisplayColor = "Green";
                        break;
                    case "Busy":
                        DisplayColor = "Yellow";
                        break;
                    case "Down":
                        DisplayColor = "Red";
                        break;
                    default:
                        DisplayColor = "Gray";
                        break;
                }
            }
        }
        public string DisplayColor
        {
            set
            {
                if (SetProperty(ref displayColor, value)) this.displayColor = value;
            }
            get => this.displayColor;
        }
    }
}
