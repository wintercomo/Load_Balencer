using System.Collections.Generic;

namespace LoadBalencerClassLibrary
{
    public interface IAlgorithm
    {
        Server GetBestServer(List<Server> allServers, int serverPort = 0);
    }
}