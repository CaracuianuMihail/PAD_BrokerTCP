using Common;
using Subscriber;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Subscriber");

        string topic = "";

        Console.WriteLine("Enter the topic: ");
        topic = Console.ReadLine().ToLower();

        var subscriberSocket = new SubscriberSocket(topic);

        subscriberSocket.Connect(Settings.BrokerIp, Settings.BrokerPort);

        Console.WriteLine("Press any key to stop Subscriber");
        Console.ReadLine();

        subscriberSocket.Stop();

        Console.WriteLine("Subscriber stopped. Press any key to exit.");
        Console.ReadKey();
    }
}