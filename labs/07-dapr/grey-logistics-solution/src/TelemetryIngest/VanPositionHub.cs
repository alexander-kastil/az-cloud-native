using Microsoft.AspNetCore.SignalR;

namespace TelemetryIngest;

public class VanPositionHub(ConnectedClients clients) : Hub
{
    public override Task OnConnectedAsync()
    {
        clients.Connected();
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        clients.Disconnected();
        return base.OnDisconnectedAsync(exception);
    }
}
