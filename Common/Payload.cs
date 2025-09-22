using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Common
{
    public class Payload
    {
        public string Topic { get; set; }
        public string Message { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToXml()
        {
            var xmlSerializer = new XmlSerializer(typeof(Payload));
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, this);
                return stringWriter.ToString();
            }
        }

        public static Payload FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Payload>(jsonString);
        }

        public static Payload FromXml(string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(Payload));
            using (var stringReader = new StringReader(xmlString))
            {
                return (Payload)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}
