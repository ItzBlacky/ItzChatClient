using System;
using WebSocketSharp;

namespace ItzChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var ws = new WebSocket("ws://localhost:811/chat"))
            {
                ws.OnOpen += (sender, e) =>
                {
                    Console.WriteLine("Socket Opened");
                };
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                ws.Connect();
                Message message = new Message("REGISTER", new string[]{ "string1", "string2", "string3" } );
                ws.Send(message.toJson());
                Console.ReadKey();
            }
        }
    }
}
