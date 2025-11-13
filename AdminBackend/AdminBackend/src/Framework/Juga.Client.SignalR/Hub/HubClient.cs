using Juga.Client.SignalR.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Juga.Client.SignalR.Hub;

public class HubClient : IHubClient
{
    private readonly HubConnection _hubConnection;

    public event Action<object> OnBroadcastAction;

    public HubClient(HubConnection hubConnection)
    {
        _hubConnection = hubConnection;
        if(_hubConnection.State == HubConnectionState.Disconnected)
            _hubConnection.StartAsync();
        ConnectAsync();
    }

    private void ConnectAsync()
    {
        _hubConnection.On<object>("ReceiveData", OnBroadcast);
    }

    public async Task Send(object data)
    {
        await _hubConnection.InvokeAsync("SendData", data);
    }

    private void OnBroadcast(object data)
    {
        OnBroadcastAction?.Invoke(data);
    }
}