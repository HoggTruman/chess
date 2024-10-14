// See https://aka.ms/new-console-template for more information
using NetworkShared;
using Server;
using System.Net;
using System.Net.Sockets;



IPEndPoint ipEndPoint = new(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
TcpListener tcpListener = new(ipEndPoint);

GameServer server = new(tcpListener);
server.Start();

Console.WriteLine("Press key to shut down");
Console.ReadKey();
await server.ShutDown();

