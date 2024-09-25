﻿using NetworkShared;
using NetworkShared.Enums;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

public class GameServer
{
    public bool IsListening { get; private set; }
    public Dictionary<int, Client> Clients = [];

    private readonly TcpListener _tcpListener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _token;
    private readonly ConcurrentBag<Task> _clientTasks = [];



    public GameServer()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Parse(ServerInfo.ipAddress), ServerInfo.port);
        _tcpListener = new TcpListener(ipEndPoint);
        _token = _cancellationTokenSource.Token;
    }


    public async Task Start()
    {
        _tcpListener.Start();
        IsListening = true;

        while (_token.IsCancellationRequested == false)
        {
            Console.WriteLine("Server is listening for connections...");
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(_token);

            Client client = new(tcpClient);
            Clients[client.Id] = client;

            Console.WriteLine($"[{DateTime.Now}] Client connected with IP {tcpClient.Client.RemoteEndPoint}");

            _clientTasks.Add(StartClientCommunications(client));
        }
    }


    public async void ShutDown()
    {
        if (_token.IsCancellationRequested)
        {
            return;
        }

        _cancellationTokenSource.Cancel();

        try
        {
            await Task.WhenAll(_clientTasks);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"{nameof(OperationCanceledException)} thrown");
        }

        _tcpListener.Stop();
        IsListening = false;

        _tcpListener.Dispose();
        _cancellationTokenSource.Dispose();

        foreach (var client in Clients.Values)
        {
            client.TcpClient.Dispose();
        }
        
    }


    private async Task StartClientCommunications(Client client)
    {
        await SendConnectedMessage(client);

        while (client.TcpClient.Connected && _token.IsCancellationRequested == false)
        {
            string message = await ReadClientMessage(client.Stream);
            await HandleClientMessage(message);
        }
    }


    private async Task SendConnectedMessage(Client client)
    {
        int msgCode = (int)ServerMessage.Connected;
        string msg = $"{msgCode}{client.Id}";
        byte[] outMsg = Encoding.UTF8.GetBytes(msg);

        await client.Stream.WriteAsync(outMsg, _token);
    }


    private async Task<string> ReadClientMessage(NetworkStream stream)
    {
        byte[] buffer = new byte[256];
        
        string message = "";
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, _token)) > 0)
        {
            message += Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (stream.DataAvailable == false)
            {
                break;
            }
        }

        return message;
    }


    private async Task HandleClientMessage(string message)
    {
        Console.WriteLine(message);
    }
    
}
