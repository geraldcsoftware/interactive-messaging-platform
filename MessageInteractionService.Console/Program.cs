// See https://aka.ms/new-console-template for more information

using MessageInteractionService.Console;

var apiClient = new MessagingApiClient();

Console.WriteLine("Welcome to the sample app for interacting with the messaging service\n\n");
var @continue = true;
while (@continue)
{
    await BeginSession(apiClient);

    Console.WriteLine("Press enter to continue..., or any other key to exit");
    var key = Console.ReadKey();
    if (key is { Key: ConsoleKey.Enter })
    {
        @continue = true;
        Console.WriteLine("You will now start another session");
    }
    else
    {
        @continue = false;
        Console.WriteLine("Program will now exit.");
    }
}


async Task BeginSession(MessagingApiClient client)
{
    Console.WriteLine("First things first, enter your sender Id here-e.g. phone or email:");
    var senderId = Console.ReadLine();
    while (string.IsNullOrWhiteSpace(senderId))
    {
        Console.WriteLine("Come on, give me something valid hey.");
        senderId = Console.ReadLine();
    }

    Console.WriteLine($"You are interacting with the service as `{senderId}`\n" +
                      $"Type what you be expecting to send to the service to start.\n\n");

    var terminate = false;
    var response = string.Empty;
    while (terminate == false)
    {
        var content = Console.ReadLine();
        Console.WriteLine("..............................................................");
        (response, terminate) = await client.SendRequest(content, senderId);

        Console.WriteLine(response);
        Console.WriteLine("==============================================================\n");

        if (terminate)
        {
            Console.WriteLine("This session has been terminated.\n");
        }
    }
}