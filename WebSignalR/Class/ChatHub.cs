using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WebSignalR.Models;

namespace WebSignalR.Class
{
    //SignalR paket se po difoltu nalazi u Asp.Net Core projektu - ukoliko ga nema treba ga instalirati preko NuGet-a
    public class ChatHub : Hub //nasledjujemo klasu Hub koja se nalazi u Microsoft.AspNetCore.SignalR
    {
        //metoda koju ce klijent da poziva, imace parametre name (ime ko salje) i text - tekst poruke
        public async Task SendMessage(string name, string text)
        {
            //objekat koji se kreira iz ChatMessage modela koji se nalazi u Models folderu
            var message = new ChatMessage 
            {
                SenderName = name,
                Text = text,
                SentAt = DateTimeOffset.UtcNow
            };

            //prenosenje poruke Svim klijentima
            await Clients.All.SendAsync(
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
    }
}
