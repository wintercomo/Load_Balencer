using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalencerClassLibrary
{
    public class StreamReader
    {

        byte[] placeholderBytes = new byte[120];
        public StreamReader()
        {
            //placeholderBytes = File.ReadAllBytes(@"Assets\Placeholder.png");
        }
        public async Task WriteMessageWithBufferAsync(NetworkStream destinationStream, byte[] messageBytes, int buffer)
        {
            if (messageBytes == null) return;
            int index = 0;
            while (index < messageBytes.Length)
            {
                int remainingBytes = messageBytes.Length - index;
                if (remainingBytes < buffer) await destinationStream.WriteAsync(messageBytes, index, remainingBytes);
                else await destinationStream.WriteAsync(messageBytes, index, buffer);
                index += buffer;
            }
        }
        public async Task<byte[]> GetBytesFromReading(int bufferSize, NetworkStream stream)
        {
            byte[] buffer = new byte[bufferSize];
            //use memory stream to save all bytes
            using (MemoryStream memory = new MemoryStream())
            {
                do
                {
                    int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    await memory.WriteAsync(buffer, 0, readBytes);
                } while (stream.DataAvailable);
                memory.Dispose();
                return memory.ToArray();
            }
        }
    }
}
