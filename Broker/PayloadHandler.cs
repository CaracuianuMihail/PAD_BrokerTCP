using Common;
using Newtonsoft.Json;
using System.Text;

namespace Broker
{
    public class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes, ConnectionInfo connInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            if (payloadString.StartsWith("subscriber#"))
            {
                connInfo.Topic = payloadString.Split("subscriber#").LastOrDefault();
                ConnectionsStorage.Add(connInfo);
            }
            else
            {
                Payload payload = null;
                if (payloadString.TrimStart().StartsWith("{"))
                {
                    // JSON detected
                    payload = Payload.FromJson(payloadString);
                }
                else if (payloadString.TrimStart().StartsWith("<"))
                {
                    // XML detected
                    payload = Payload.FromXml(payloadString);
                }

                if (payload != null)
                {
                    PayloadStorage.Add(payload);
                }
            }

            Console.WriteLine(payloadString);
        }
    }
}
