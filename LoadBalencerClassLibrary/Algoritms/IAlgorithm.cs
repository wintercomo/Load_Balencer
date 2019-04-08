using System.Collections.Generic;

namespace LoadBalencerClassLibrary
{
    public interface IAlgorithm
    {
        string Name();
        Server GetBestServer(List<Server> allServers);
    }
}