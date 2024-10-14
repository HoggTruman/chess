// See https://aka.ms/new-console-template for more information
using Server;



GameServer server = new();
server.Start();

Console.WriteLine("Press key to shut down");
Console.ReadKey();
await server.ShutDown();

