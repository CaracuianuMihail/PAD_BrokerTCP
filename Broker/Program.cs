using Broker;
using Common;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Broker");

        BrokerSocket socket = new BrokerSocket();
        socket.Start(Settings.BrokerIp, Settings.BrokerPort);

        var worker = new MessageWorker();

        Task.Factory.StartNew(worker.DoSendMessageWork, TaskCreationOptions.LongRunning);

        Console.ReadLine();
    }
}