# Chess
A networked Chess game featuring:
* An interactive chess experience with full gameplay logic, including special moves (e.g. Castling, En Passant, Promotion)
* A client WPF application to play local and online games.
* A server application to connect clients and communicate gameplay updates.
* Game logic tests
* Client / Server integration tests

![Alt text](./preview.png?raw=true "Preview")

# Technology Used
* C# / .NET 8.0
* [TCPClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-8.0) / [TCPListener](https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-8.0)
* [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/?view=netdesktop-8.0)
* [xUnit](https://xunit.net/) + [Moq](https://www.nuget.org/packages/Moq/)
* Async
* Events