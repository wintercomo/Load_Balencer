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
using LoadBalencerClassLibrary.DataModels;

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
        public void StartHealthChecker()
        {
            var timer = new System.Threading.Timer(
            async e => await CheckServerStatusAsync(allServers),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(100));


        }
        public async Task<ObservableCollection<Server>> CheckServerStatusAsync(ObservableCollection<Server> allServers)
        {
            foreach (var server in allServers)
            {
                TcpClient tcpClient = await TryConnect(server);
                if (tcpClient == null) server.Status = "Not running";
                else
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("GET / HTTP/1.1");
                    builder.AppendLine($"Host: {server.ServerURL}");
                    builder.AppendLine("Connection: close");
                    builder.AppendLine();
                    var header = Encoding.ASCII.GetBytes(builder.ToString());
                    await tcpClient.GetStream().WriteAsync(header, 0, header.Length);
                    Stopwatch watch = Stopwatch.StartNew();
                    await streamReader.GetBytesFromReading(2024, tcpClient.GetStream());
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    UpdateServerStatus(server, elapsedMs);
                }
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
                    Server currentServer = GetBestServer();
                    if (cookieInfo != null)
                    {
                        string[] cookieParams = cookieInfo.Split(',');
                        var wantedServer = GetBestServer(cookieParams);
                        if (wantedServer != null) currentServer = wantedServer;
                    }
                    await HandleServerResponse(bufferSize, clientStream, requestBytes, currentServer);
                }
            }
        }

        private async Task HandleServerResponse(int bufferSize, NetworkStream clientStream, byte[] requestBytes, Server currentServer)
        {
            if (currentServer == null) return;
            byte[] responseBytes = await GetServerResponseAsync(currentServer, requestBytes);
            HttpRequest responseObject = new HttpRequest(ASCIIEncoding.ASCII.GetString(responseBytes));
            Session session = new Session(currentServer.Port);
            currentServer.Sessions.Add(session);
            if (currentServer.Port != session.ServerPort)
            {
                responseObject.UpdateHeader("Set-Cookie", $"server = {currentServer.Port}, Session ={session.SessionId}");
            }
            responseBytes = ASCIIEncoding.ASCII.GetBytes(responseObject.HttpString);
            await streamReader.WriteMessageWithBufferAsync(clientStream, responseBytes, bufferSize);
        }

        private async Task<byte[]> GetServerResponseAsync(Server server, byte[] requestBytes)
        {
            try
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
            //if server cannot connect then pick a different server
            catch (Exception)
            {
                server.Status = "Not Running";
                return ASCIIEncoding.ASCII.GetBytes($"Server {server.ServerURL} on port {server.Port} was not reachable.\r\n" +
                    $"Refresh to generate a new cookie and session"); // return empty byte array to send
            }
        }

        private Server GetBestServer(string[] cookieParams = null)
        {
            List<Server> onlineServers = allServers.Where(server => server.Status != "Not running").ToList();
            IAlgorithm currentAlgorithm = loadBalencerViewModel.SelectedAlgorithm;
            if (cookieParams != null) return currentAlgorithm.GetBestServer(onlineServers, cookieParams);
            return currentAlgorithm.GetBestServer(onlineServers);
        }
    }
}
