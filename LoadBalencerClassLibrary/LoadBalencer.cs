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
using System.Threading;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerServer
    {
        public LoadBalencerServer(LoadBalencerViewModel model)
        {
            this.AllServers = model.Servers;
            this.LoadBalencerViewModel = model;
            this.StreamReader = new StreamReader();
        }
        public ObservableCollection<Server> AllServers { get; }
        public LoadBalencerViewModel LoadBalencerViewModel { get; }

        public StreamReader StreamReader { get; }

        public TcpListener StartAServer(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return listener;
        }
        public void StartHealthChecker()
        {
            var _timer = new Timer(
            async e => await CheckServerStatusAsync(AllServers),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
        }
        public async Task CheckServerStatusAsync(ObservableCollection<Server> allServers = null)
        {
            Console.WriteLine("[NOTICE] Checking health!");
            foreach (var server in allServers.ToList())
            {
                try
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
                        await StreamReader.GetBytesFromReading(2024, tcpClient.GetStream());
                        watch.Stop();
                        long elapsedMs = watch.ElapsedMilliseconds;
                        UpdateServerStatus(server, elapsedMs);
                    }
                }
                catch (Exception)
                {
                    server.Status = "Not running";
                    continue;
                }
            }
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
                return null; // null is expected if connection connot be made
            }
        }

        public async Task HandleHttpRequest(TcpListener tcplistener)
        {
            using (TcpClient tcpClient = await tcplistener.AcceptTcpClientAsync())
            {
                using (NetworkStream clientStream = tcpClient.GetStream())
                {
                    byte[] requestBytes = await StreamReader.GetBytesFromReading(1024, clientStream);
                    HttpRequest requestObject = new HttpRequest(ASCIIEncoding.ASCII.GetString(requestBytes));
                    var cookieInfo = requestObject.GetHeader("Cookie");
                    string[] cookieParams = null;
                    if (cookieInfo != null) cookieParams = cookieInfo.Split(',');
                    var currentServer = GetBestServer(cookieParams);
                    if (currentServer != null) await HandleServerResponse(clientStream, requestBytes, currentServer, cookieInfo);
                    else await StreamReader.WriteMessageWithBufferAsync(clientStream, ASCIIEncoding.ASCII.GetBytes("All servers offline"), 1024);
                }
            }
        }

        private async Task HandleServerResponse(NetworkStream clientStream, byte[] requestBytes, Server currentServer, string cookieInfo)
        {
            byte[] responseBytes = await GetServerResponseAsync(currentServer, requestBytes);
            HttpRequest responseObject = new HttpRequest(ASCIIEncoding.ASCII.GetString(responseBytes));
            if (cookieInfo != null 
                && (LoadBalencerViewModel.SelectedAlgorithm.GetType() == new SessionBasedAlgorithm().GetType() 
                || LoadBalencerViewModel.SelectedAlgorithm.GetType() == new CookieBasedAlgorithm().GetType()))
                    responseBytes = SetSession(currentServer, responseObject);
            await StreamReader.WriteMessageWithBufferAsync(clientStream, responseBytes, 1024);
        }

        private static byte[] SetSession(Server currentServer, HttpRequest responseObject)
        {
            byte[] responseBytes;
            Session session = new Session(currentServer.Port);
            currentServer.Sessions.Add(session);
            responseObject.UpdateHeader("Set-Cookie", $"server = {currentServer.Port}, Session ={session.SessionId}");
            responseBytes = ASCIIEncoding.ASCII.GetBytes(responseObject.HttpString);
            return responseBytes;
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
                        var responseBytes = await StreamReader.GetBytesFromReading(1024, clientStream);
                        return responseBytes;
                    }
                }
            }
            //if server cannot connect then send response that server is down
            catch (Exception)
            {
                server.Status = "Not running";
                return ASCIIEncoding.ASCII.GetBytes($"Server {server.ServerURL} on port {server.Port} was not reachable. Refresh to generate a new cookie and session");
            }
        }

        private Server GetBestServer(string[] cookieParams = null)
        {
            List<Server> onlineServers = AllServers.Where(server => server.Status != "Not running").ToList();
            IAlgorithm currentAlgorithm = LoadBalencerViewModel.SelectedAlgorithm;
            return currentAlgorithm.GetBestServer(onlineServers, cookieParams);
        }
    }
}
