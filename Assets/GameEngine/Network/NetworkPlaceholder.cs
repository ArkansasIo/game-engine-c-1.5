using System;
using System.Collections.Generic;
namespace GameEngine.Network
{
    /// <summary>
    /// NetworkManager handles network communication, multiplayer, and data transfer.
    /// </summary>
    public class NetworkManager
    {
        private List<string> connectedClients = new List<string>();

        public void Connect(string clientId)
        {
            connectedClients.Add(clientId);
            Console.WriteLine($"Client {clientId} connected.");
        }

        public void Disconnect(string clientId)
        {
            connectedClients.Remove(clientId);
            Console.WriteLine($"Client {clientId} disconnected.");
        }

        public void SendData(string data)
        {
            Console.WriteLine($"Data sent: {data}");
        }

        public void ReceiveData()
        {
            Console.WriteLine("Data received.");
        }
    }
}
