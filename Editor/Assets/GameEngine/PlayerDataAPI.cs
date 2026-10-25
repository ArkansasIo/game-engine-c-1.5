namespace GameEngine
{
    public interface IPlayerDataAPI
    {
        void SavePlayerData(string playerId, object data);
        object LoadPlayerData(string playerId);
        // Add more data methods as needed
    }
}
