using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSignalR.Models;

namespace WebSignalR.Services
{
    public class InMemoryChatRoomService : IChatRoomService
    {
        private readonly Dictionary<Guid, ChatRoom> _roomInfo = new Dictionary<Guid, ChatRoom>();
        private readonly Dictionary<Guid, List<ChatMessage>> _messageHistory = new Dictionary<Guid, List<ChatMessage>>();
        public Task<Guid> CreateRoom(string connectionId)
        {
            var id = Guid.NewGuid();
            _roomInfo[id] = new ChatRoom
            {
                OwnerConnectionId = connectionId
            };
            return Task.FromResult(id);
        }

        public Task<Guid> GetRoomForConnectionId(string connectionId)
        {
            var foundRoom = _roomInfo.FirstOrDefault(c => c.Value.OwnerConnectionId == connectionId);
            if(foundRoom.Key == Guid.Empty)
                throw new ArgumentException("Nepostojeci Connection ID.");
            return Task.FromResult(foundRoom.Key);
        }

        public Task SetRoomName(Guid roomId, string name)
        {
            if (!_roomInfo.ContainsKey(roomId))
                throw new ArgumentException("Nedozvoljen ID sobe...");
            _roomInfo[roomId].Name = name;
            return Task.CompletedTask;
        }

        public Task AddMessage(Guid roomId, ChatMessage message)
        {
            if(!_messageHistory.ContainsKey(roomId))
                _messageHistory[roomId] = new List<ChatMessage>();

            _messageHistory[roomId].Add(message);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ChatMessage>> GetMessageHistory(Guid roomId)
        {
            _messageHistory.TryGetValue(roomId, out var messages);

            messages = messages ?? new List<ChatMessage>();
            var sortedMessages = messages.OrderBy(c => c.SentAt).AsEnumerable();

            return Task.FromResult(sortedMessages);
        }
    }
}
