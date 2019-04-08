using System;
using System.Diagnostics;
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
        StreamReader streamReader = new StreamReader();
        ObservableCollection<Server> allServers;
        LoadBalencerViewModel model;
        public LoadBalencerServer(ObservableCollection<Server> allServers, LoadBalencerViewModel model)
        {
            this.allServers = allServers;
            this.model = model;
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
                if (proxyTcpClient == null) server.Status = "Not running";
                else server.Status = "Normal";
            }
            return servers;
        }
        private static void UpdateServerStatus(Server server, long elapsedMs)
        {
            if (elapsedMs > 3000)
            {
                server.Status = "Down";
            }
            else if (elapsedMs > 1000)
            {
                server.Status = "Busy";
            }
            else
            {
                server.Status = "Normal";
            }
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
                    Server currentServer = getBestServer();
                    Stopwatch watch = Stopwatch.StartNew();
                    byte[] responseBytes = await GetServerResponseAsync(currentServer, requestBytes);
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    UpdateServerStatus(currentServer, elapsedMs);
                    await streamReader.WriteMessageWithBufferAsync(clientStream, responseBytes, bufferSize);
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

        private Server getBestServer()
        {
            List<Server> onlineServers = allServers.Where(server => server.Status == "Normal").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Busy").ToList();
            if (onlineServers.Count == 0) onlineServers = allServers.Where(server => server.Status == "Down").ToList();
            //For now take the first one
            IAlgorithm currentAlgorithm = model.GetCurrentItem();
            return currentAlgorithm.GetBestServer(onlineServers);
        }
    }
}
