using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebSignalR.Models;
using WebSignalR.Services;

namespace WebSignalR.Class
{
    //SignalR paket se po difoltu nalazi u Asp.Net Core projektu - ukoliko ga nema treba ga instalirati preko NuGet-a
    public class ChatHub : Hub //nasledjujemo klasu Hub koja se nalazi u Microsoft.AspNetCore.SignalR
    {
        private readonly IChatRoomService _chatRoomService;

        public ChatHub(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        public override async Task OnConnectedAsync()
        {
            var roomId = await _chatRoomService.CreateRoom(Context.ConnectionId);
            await Groups.AddToGroupAsync(
                Context.ConnectionId, roomId.ToString());

            await Clients.Caller.SendAsync(
                "ReceiveMessage",
                "WebSignalR",
                DateTimeOffset.UtcNow,
                "Pozdrav...");
            await base.OnConnectedAsync();
        }

        //metoda koju ce klijent da poziva, imace parametre name (ime ko salje) i text - tekst poruke
        public async Task SendMessage(string name, string text)
        {
            var roomId = _chatRoomService.GetRoomForConnectionId(Context.ConnectionId);

            //objekat koji se kreira iz ChatMessage modela koji se nalazi u Models folderu
            var message = new ChatMessage 
            {
                SenderName = name,
                Text = text,
                SentAt = DateTimeOffset.UtcNow
            };

            await _chatRoomService.AddMessage(roomId, message);

            //prenosenje poruke Svim klijentima
            await Clients.Group(roomId.ToString()).SendAsync(
                "ReceiveMessage", //ime metode  koju pozivamo na klijentu i prosledjujemo sve parametre koje saljemo klijentu
                message.SenderName, 
                message.SentAt, 
                message.Text);
        }

        //klijente kreiramo pomocu SignalR JavaScript Client biblioteke
        //to sam konfigurisao u wwwroot / js/ index.js
        //takodje u Pages/Index moramo dodati chat
        //i na kraju moramo dodati (na strani klijenta) SignalR i konektovati ga sa Hub-om
        //SignalR javascript klijent je objavljen na NPM - ako koristite NPM ili neki drugi klijent paket menadzer u projektu mozete instalirati ovog klijenta
        //i spakovati ga kao bilo koji JavaScriptfajl ili biblioteku

        public async Task SetName(string visitorName)
        {
            var roomName = $"Chat with {visitorName} from the web";

            var roomId = await _chatRoomService.GetRoomForConnectionId(
                Context.ConnectionId);

            await _chatRoomService.SetRoomName(roomId, roomName);
        }
    }
}
