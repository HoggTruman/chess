﻿using System.Net.Sockets;

namespace Server;

public class Client
{
    private static int _count = 0;
    

    public int Id { get; }

    public TcpClient TcpClient { get; }
    public NetworkStream Stream { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    public CancellationToken Token { get; }

    public Task ServerCommunications { get; set; }

    public int RoomId { get; set; }

    public Client(TcpClient tcpClient, CancellationTokenSource cancellationTokenSource, Func<Client, Task> startCommunications)
    {
        Id = ++_count;
        TcpClient = tcpClient;
        Stream = tcpClient.GetStream();
        CancellationTokenSource = cancellationTokenSource;
        Token = CancellationTokenSource.Token;
        ServerCommunications = startCommunications(this);
    }
}

