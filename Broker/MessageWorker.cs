using Newtonsoft.Json;
using System.Text;

namespace Broker
{
    public class MessageWorker
    {
        private const int TimeToSleep = 500;

        public void DoSendMessageWork()
        {
            while (true)
            {

                while (!PayloadStorage.IsEmpty())
                {
                    var payload = PayloadStorage.GetNext();

                    if(payload != null)
                    {
                        var connections = ConnectionsStorage.GetConnectionsByTopic(payload.Topic);

                        foreach (var connection in connections)
                        {
                            var payloadString = JsonConvert.SerializeObject(payload);
                            byte[] data = Encoding.UTF8.GetBytes(payloadString);

                            connection.Socket.Send(data);
                        }
                    }
                }

                Thread.Sleep(TimeToSleep);
            }
        }
    }
}
