using Common;
using System.Text;

namespace Subscriber
{
    public class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            Payload payload = null;

            if (payloadString.TrimStart().StartsWith("{"))
            {
                payload = Payload.FromJson(payloadString);
            }
            else if (payloadString.TrimStart().StartsWith("<"))
            {
                payload = Payload.FromXml(payloadString);
            }

            if (payload != null)
            {
                Console.WriteLine(payload.Message);
            }
        }
    }
}
