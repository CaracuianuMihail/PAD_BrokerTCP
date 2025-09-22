using Common;
using System.Collections.Concurrent;

namespace Broker
{
    static class PayloadStorage
    {
        private static ConcurrentQueue<Payload> _payloadsQuene;

        static PayloadStorage()
        {
            _payloadsQuene = new ConcurrentQueue<Payload>();
        }

        public static void Add(Payload payload)
        {
            _payloadsQuene.Enqueue(payload);
        }

        public static Payload GetNext()
        {
            Payload payload = null;

            _payloadsQuene.TryDequeue(out payload);

            return payload;
        }

        public static bool IsEmpty()
        {
            return _payloadsQuene.IsEmpty;
        }
    }
}
