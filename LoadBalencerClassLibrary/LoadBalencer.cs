using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerServer
    {
        TcpListener listener;
        StreamReader streamReader = new StreamReader();
        ObservableCollection<Server> allServers;
        List<Algorithm> algorithms;

        public LoadBalencerServer(ObservableCollection<Server> allServers, List<Algorithm> algorithms)
        {
            this.allServers = allServers;
            this.algorithms = algorithms;
        }

        public TcpListener StartAServer(int port)
        {
            Console.WriteLine(port);
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();
            return listener;
        }
        public async Task<ObservableCollection<Server>> CheckServerStatusAsync(ObservableCollection<Server> servers)
        {

            foreach (var server in servers)
            {
                TcpClient proxyTcpClient = await TryConnect(server);
                if (proxyTcpClient == null)
                {
                    server.Status = "Down";
                }
                else
                {
                    server.Status = "Normal";
                }
            }
            return servers;
        }

        private async Task<TcpClient> TryConnect(Server server)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(server.ServerURL, server.Port);
                return tcpClient;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task HandleHttpRequest(TcpListener tcplistener, int bufferSize)
        {
            using (TcpClient tcpClient = await tcplistener.AcceptTcpClientAsync())
            {

                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    byte[] requestBytes = await streamReader.GetBytesFromReading(bufferSize, clientStream);
                    string requestInfo = ASCIIEncoding.ASCII.GetString(requestBytes, 0, requestBytes.Length);
                    HttpRequest request = new HttpRequest(requestInfo);

                    IEnumerable<Server> onlineServers = allServers.Where(server => server.Status == "Normal");
                    Server bestServer = getBestServer(onlineServers);

                    byte[] responseBytes = await GetServerResponseAsync(bestServer, requestBytes);
                    ///var responseBytes = await streamReader.MakeProxyRequestAsync(h1, bufferSize);
                    await streamReader.WriteMessageWithBufferAsync(clientStream, responseBytes, bufferSize);
                    clientStream.Dispose();
                    tcpClient.Dispose();
                }
            }
        }
        private async Task<byte[]> GetServerResponseAsync(Server server, byte[] requestBytes)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(server.ServerURL, server.Port);
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    await clientStream.WriteAsync(requestBytes, 0, requestBytes.Length);
                    var responseBytes = await streamReader.GetBytesFromReading(1024, clientStream);
                    return responseBytes;
                }
            }
        }

        private Server getBestServer(IEnumerable<Server> onlineServers)
        {
            //For now take the first one
            return onlineServers.First();
        }

        public void Start(int port)
        {

        }
    }
}
