namespace GameEngine
{
    public interface IGameAPI
    {
        void Login(string username, string password);
        void Logout();
        void SendChat(string message);
        void RequestPartyInvite(string playerId);
        // Add more API methods as needed
    }
}
