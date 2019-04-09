using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary.DataModels
{
    public class Session
    {
        Guid sessionId;
        readonly int serverPort;

        public Session(int serverPort)
        {
            this.SessionId = Guid.NewGuid();
            this.serverPort = serverPort;
        }

        public Guid SessionId { get => sessionId; set => sessionId = value; }

        public int ServerPort => serverPort;
    }
}
