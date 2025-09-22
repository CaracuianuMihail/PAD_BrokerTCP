using System.Net.Sockets;

namespace Common
{
    public class ConnectionInfo
    {
        public const int BufferSize = 2048;
        public Socket Socket {  get; set; }
        public string Address { get; set; }
        public string Topic { get; set; }
        public byte[] Data { get; set; }

        public ConnectionInfo()
        {
            Data = new byte[BufferSize];
        }
    }
}
