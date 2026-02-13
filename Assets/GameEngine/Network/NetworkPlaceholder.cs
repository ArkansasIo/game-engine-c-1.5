namespace GameEngine.Network
{
    public class NetworkPlaceholder
    {
        public void SendData(string data) {
            System.Console.WriteLine($"Data sent: {data} (stub).");
        }
        public void ReceiveData() {
            System.Console.WriteLine("Data received (stub).");
        }
    }
}
