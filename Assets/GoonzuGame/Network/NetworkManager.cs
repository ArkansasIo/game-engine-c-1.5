using System;

namespace GoonzuGame.Network
{
    public class NetworkManager
    {
        public bool IsConnected { get; set; }

        public void Connect()
        {
            IsConnected = true;
            Console.WriteLine("Connected to server.");
        }

        public void SendData(string data)
        {
            if (IsConnected)
                Console.WriteLine($"Sent data: {data}");
            else
                Console.WriteLine("Not connected.");
        }
    }
}
