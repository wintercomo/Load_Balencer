using System;
using System.Collections.Generic;
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

        public TcpListener StartAServer(int port)
        {
            Console.WriteLine(port);
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            return listener;
        }
        public async Task HandleHttpRequest(TcpListener tcplistener, int bufferSize)
        {
            while (true)
            {
                TcpClient tcpClient = await tcplistener.AcceptTcpClientAsync();
                NetworkStream clientStream = tcpClient.GetStream();
                byte[] requestBytes = await streamReader.GetBytesFromReading(bufferSize, clientStream);
                string requestInfo = ASCIIEncoding.ASCII.GetString(requestBytes, 0, requestBytes.Length);
                HttpRequest h1 = new HttpRequest(requestInfo);
                var responseBytes = await streamReader.MakeProxyRequestAsync(h1, bufferSize);
                await streamReader.WriteMessageWithBufferAsync(clientStream, responseBytes, bufferSize);
                clientStream.Dispose();
                tcpClient.Dispose();
            }
        }
        public void Start(int port)
        {
            
        }
    }
}
