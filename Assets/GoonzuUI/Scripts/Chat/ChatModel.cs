using System.Collections.Generic;

namespace GoonzuUI.Chat
{
    public sealed class ChatModel
    {
        private readonly Dictionary<string, List<string>> _byChannel = new();

        public IReadOnlyList<string> GetLines(string channel)
        {
            if (_byChannel.TryGetValue(channel, out var list)) return list;
            return System.Array.Empty<string>();
        }

        public void Append(string channel, string line, int maxLines = 200)
        {
            if (!_byChannel.TryGetValue(channel, out var list))
            {
                list = new List<string>(maxLines);
                _byChannel[channel] = list;
            }

            list.Add(line);
            if (list.Count > maxLines) list.RemoveAt(0);
        }
    }
}
