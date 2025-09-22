using Common;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Subscriber
{
    public class SubscriberSocket
    {
        private Socket _socket;
        private string _topic;
        private bool _isConnected = false;
        private int _retryInterval = 5000;
        private bool _keepRunning = true;

        public SubscriberSocket(string topic)
        {
            _topic = topic;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            Console.WriteLine("Trying to connect --");
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
        }

        private void ConnectedCallback(IAsyncResult asyncResult)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Subscriber Connected to broker");
                Subscribe();
                StartRecieve();
                _isConnected = true;
            }
            else
            {
                Console.WriteLine("Error: Could not connect to broker");
                AttempReconnect();
            }
        }

        private void Subscribe()
        {
            var data = Encoding.UTF8.GetBytes("subscriber#" + _topic);
            Send(data);
        }

        private void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not send data: " + ex.ToString());
            }
        }

        private void StartRecieve()
        {

            ConnectionInfo connection = new ConnectionInfo();
            connection.Socket = _socket;

            _socket.BeginReceive(connection.Data, 0 , connection.Data.Length, SocketFlags.None, ReceiveCallback, connection);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                SocketError response;
                int buffSize = _socket.EndReceive(asyncResult, out response);

                if(response == SocketError.Success && buffSize > 0)
                {
                    byte[] payloadBytes = new byte[buffSize];
                    Array.Copy(connectionInfo.Data, payloadBytes, payloadBytes.Length);

                    PayloadHandler.Handle(payloadBytes);
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cant receive data from broker! :" + ex.Message);
                AttempReconnect();
            }

            finally
            {
                try
                {
                    if (_isConnected)
                    {
                        connectionInfo.Socket.BeginReceive(connectionInfo.Data, 0, connectionInfo.Data.Length, SocketFlags.None, ReceiveCallback, connectionInfo);
                    }
                }
                catch(Exception ex) 
                {
                    Console.WriteLine("Connection closed: " + ex.Message);
                    //connectionInfo.Socket.Close();
                    AttempReconnect();
                }
            }
        }

        private void AttempReconnect()
        {
            if(!_keepRunning) return; 

            Console.WriteLine($"Attempting to reconnect in {_retryInterval / 1000} seconds...");
            _isConnected = false;

            Thread.Sleep(_retryInterval);

            _socket.Close();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Connect(Settings.BrokerIp, Settings.BrokerPort);
        }

        public void Stop()
        {
            _keepRunning = false;
            _socket.Close();
        }
    }
}
