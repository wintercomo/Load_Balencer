using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LoadBalencerClassLibrary.Algoritms;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerServer
    {
        StreamReader streamReader = new StreamReader();
        ObservableCollection<Server> allServers;
        LoadBalencerViewModel loadBalencerViewModel;
        public LoadBalencerServer(LoadBalencerViewModel model)
        {
            this.allServers = model.Servers;
            this.loadBalencerViewModel = model;
        }

        public TcpListener StartAServer(int port)
        {
            Console.WriteLine(port);
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return listener;
        }
        public async Task<ObservableCollection<Server>> CheckServerStatusAsync(ObservableCollection<Server> allServers)
        {
            foreach (var server in allServers)
            {
                TcpClient proxyTcpClient = await TryConnect(server);
                if (proxyTcpClient == null) server.Status = "Not running";
                else server.Status = "Normal";
            }
            return allServers;
        }
        private static void UpdateServerStatus(Server server, long elapsedMs)
        {
            if (elapsedMs > 3000) server.Status = "Down";
            else if (elapsedMs > 1000) server.Status = "Busy";
            else server.Status = "Normal";
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
                    HttpRequest requestObject = new HttpRequest(ASCIIEncoding.ASCII.GetString(requestBytes));
                    var cookieInfo = requestObject.GetHeader("Cookie");
                    await CheckServerStatusAsync(allServers);
                    Server currentServer = GetBestServer();
                    if (cookieInfo != null)
                    {
                        var cookieParams = cookieInfo.Split('=');
                        var wantedServer = GetBestServer(cookieParams[1]);
                        if (wantedServer != null) currentServer = wantedServer;
                    }
                    Stopwatch watch = Stopwatch.StartNew();
                    byte[] responseBytes = await GetServerResponseAsync(currentServer, requestBytes);
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    HttpRequest responseObject = new HttpRequest(ASCIIEncoding.ASCII.GetString(responseBytes));
                    responseObject.UpdateHeader("Set-Cookie", $"server = {currentServer.Port}");
                    responseBytes = ASCIIEncoding.ASCII.GetBytes(responseObject.HttpString);
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

        private Server GetBestServer(string serverPort = null)
        {
            List<Server> onlineServers = allServers.Where(server => server.Status != "Not running").ToList();
            IAlgorithm currentAlgorithm = loadBalencerViewModel.SelectedAlgorithm;
            if (serverPort != null) return currentAlgorithm.GetBestServer(onlineServers, int.Parse(serverPort));
            return currentAlgorithm.GetBestServer(onlineServers);
        }
    }
}
