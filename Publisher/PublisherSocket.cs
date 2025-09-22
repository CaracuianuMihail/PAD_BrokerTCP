using Common;
using Publisher.Enums;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Publisher
{
    public class PublisherSocket
    {
        private Socket _socket;
        public bool isConnected;
        private byte[] data;
        public SerializationFormat SerializationType { get; set; }

        public PublisherSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallBack, null);
            Thread.Sleep(2000);
        }

        //public void Stop()
        //{
        //    _isRunning = false;
        //    if(_socket.Connected)
        //    {
        //        _socket.Shutdown(SocketShutdown.Both);
        //        _socket.Close();
        //    }
        //    Console.WriteLine("Publisher connection stopped.");
        //}

        public void Send(Payload payload)
        {
            //if(!isConnected)
            //{
            //    Console.WriteLine("Publisher is not connected to the broker");
            //    return;
            //}

            //Console.WriteLine(SerializationType);
            //Console.ReadLine();
            string serializedData;

            if (SerializationType == SerializationFormat.Json)
            {
                serializedData = payload.ToJson();
            }
            else
            {
                serializedData = payload.ToXml();
            }

            data = Encoding.UTF8.GetBytes(serializedData); 
            try
            {
                _socket.Send(data);
            }
            catch(SocketException ex)
            {
                Console.Clear();
                isConnected = false;
                Thread recconectThread = new Thread(Reconnect);
                recconectThread.Start();
                recconectThread.Join();
            }
        }

        private void ConnectedCallBack(IAsyncResult result)
        {

            if (_socket.Connected)
            {
                Console.WriteLine("Sender connected to Broker");
                isConnected = true;
            }
            else
            {
                Console.WriteLine("Sender not connected to Broker");
                isConnected = false;
            }
        }

        public void Reconnect()
        {
            while (!isConnected)
            {
                Console.WriteLine("Attempting to reconnect...");
                Thread.Sleep(5000);

                try
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Connect(Settings.BrokerIp, Settings.BrokerPort);
                    if(_socket.Connected)
                    {

                        _socket.Send(data);
                    }
;                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to connect {ex}");
                }
            }
        }
    }
}
