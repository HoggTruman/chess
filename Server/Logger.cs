using System.Net.Sockets;

namespace Server;

public static class Logger
{
    #region Private Methods

    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    #endregion



    #region Public Methods

    public static void LogListening()
    {
        Log("Server is listening for connections...");
    }

    public static void LogShuttingDown()
    {
        Log("Server shutting down");
    }

    public static void LogConnectedClient(TcpClient tcpClient)
    {
        Log($"Connected client with IP {tcpClient.Client.RemoteEndPoint}");
    }

    public static void LogLostConnectionToClient(Client client)
    {
        Log($"Lost connection to client with IP {client.TcpClient.Client.RemoteEndPoint}");
    }

    public static void LogDisconnectedClient(Client client)
    {
        Log($"Disconnected client with IP {client.TcpClient.Client.RemoteEndPoint}");
    }

    public static void LogInvalidMessageReceived(Client client)
    {
        Log($"Invalid Message received from client with IP {client.TcpClient.Client.RemoteEndPoint}");
    }

    public static void LogException(Exception ex)
    {
        Log(ex.ToString());
    }

    #endregion
}
