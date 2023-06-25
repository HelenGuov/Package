// See https://aka.ms/new-console-template for more information

using EventAPIProcessor;

Console.WriteLine("View the EventProcessorServiceTest for seeing data saved to store");
Console.WriteLine("View ApiServiceTest for handling of the Scan Event response message");

var service = new EventProcessorService(new ApiService());
while (true)
{
    service.Start();
}

Console.WriteLine("Press any key to exit");
Console.ReadKey(); 