using Common;
using Publisher;
using Publisher.Enums;

public class Program
{
    private static Payload _payload = new Payload();
    private static PublisherSocket publicherSoket = new PublisherSocket();
    private static SerializationFormat currentFormat = SerializationFormat.Json;
    private static void Main(string[] args)
    {
        Console.WriteLine("Publisher");
        publicherSoket.Connect(Settings.BrokerIp, Settings.BrokerPort);

        if (publicherSoket.isConnected)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Current topic => {_payload.Topic}.");
                Console.WriteLine($"Current format => {currentFormat}.");
                Console.WriteLine("1. Select Topic.");
                Console.WriteLine("2. Publish a message.");
                Console.WriteLine("3. Switch Serialization Format (JSON/XML).");
                Console.WriteLine("4. Disconnect.");

                try
                {
                    int command = Convert.ToInt32(Console.ReadLine());
                    Menu(command);
                }
                catch
                {
                    Console.WriteLine("Inserted command is invalid.");
                    Console.ReadKey();
                }


            }
        }
    }

    private static void Menu(int command)
    {
        switch (command)
        {

            case 1:
                Console.Clear();
                Console.WriteLine("Select topic: ");
                _payload.Topic = Console.ReadLine().ToLower();
                break;

            case 2:
                //publicherSoket.CheckBrokerStatus();
                if (_payload.Topic == string.Empty || _payload.Topic == null)
                {
                    Console.WriteLine("Topic field is empty!!!");
                    Console.ReadKey();
                    break;
                }

                Console.Clear();
                Console.WriteLine("Message: ");
                _payload.Message = Console.ReadLine().ToLower();

                publicherSoket.Send(_payload);
                break;

            case 3:
                // Switch between JSON and XML
                currentFormat = currentFormat == SerializationFormat.Json ? SerializationFormat.Xml : SerializationFormat.Json;
                publicherSoket.SerializationType = currentFormat;
                Console.WriteLine($"Serialization format switched to {currentFormat}");
                Console.ReadKey();
                break;

            case 4:
                Console.Clear();
                Environment.Exit(0);
                break;

            default:
                Console.Clear();
                Console.WriteLine("Select a valid option!!!");
                break;
        }
    }
}