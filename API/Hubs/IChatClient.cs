namespace API.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(HubMessage message);
}