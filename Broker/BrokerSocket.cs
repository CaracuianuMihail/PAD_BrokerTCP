using Common;
using System.Net;
using System.Net.Sockets;

namespace Broker
{
    public class BrokerSocket
    {
        private Socket _socket;
        private const int ConnectionsLimit = 8;

        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            _socket.Listen(ConnectionsLimit);
            Accept();
        }

        private void Accept()
        {
            try
            {
                _socket.BeginAccept(AcceptedCallBack, null);
            }
            catch (SocketException ex)
            {
                HandleSocketException(ex);
            }
        }

        private void AcceptedCallBack(IAsyncResult result)
        {
            ConnectionInfo conn = new ConnectionInfo();

                try
                {
                    conn.Socket = _socket.EndAccept(result);
                    Console.WriteLine(conn.Address);
                    conn.Address = conn.Socket.RemoteEndPoint.ToString();

                Console.WriteLine($"Connection accepted from {conn.Address}");

                StartReceiving(conn);
            }
            catch (SocketException ex)
            {
                HandleSocketException(ex);
            }
            finally
            {
                Accept();
            }
        }

        private void StartReceiving(ConnectionInfo conn)
        {
            try
            {
                conn.Socket.BeginReceive(conn.Data, 0, conn.Data.Length, SocketFlags.None, ReceiveCallBack, conn);
            }
            catch (SocketException ex)
            {
                HandleSocketException(ex);
                CloseConnection(conn);
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            ConnectionInfo conn = result.AsyncState as ConnectionInfo;

            try
            {
                int buffSize = conn.Socket.EndReceive(result, out SocketError response);

                if (response == SocketError.Success && buffSize > 0)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(conn.Data, payload, payload.Length);

                    PayloadHandler.Handle(payload, conn);
                    StartReceiving(conn);
                }
                else
                {
                    CloseConnection(conn);
                }
            }
            catch (SocketException ex)
            {
                HandleSocketException(ex);
                CloseConnection(conn);
            }
        }

        private void CloseConnection(ConnectionInfo conn)
        {
            try
            {
                Console.WriteLine($"Closing connection from {conn.Address}");
                var address = conn.Socket.RemoteEndPoint.ToString();

                ConnectionsStorage.Remove(address);
                conn.Socket.Close();
                
            }
            catch (SocketException ex)
            {
                HandleSocketException(ex);
            }
        }

        private void HandleSocketException(SocketException ex)
        {

            Console.WriteLine($"Socket error: {ex.Message}");
        }
    }
}
