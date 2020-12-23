using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;

namespace ItzChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("URI to connect: ");
            string connectionString = Console.ReadLine();
            using var ws = new WebSocket(connectionString);
            string key = "";
            ws.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Socket Opened");
            };
            ws.OnMessage += (sender, e) =>
            {
                if(!e.IsText) 
                {
                    Console.WriteLine("Received non text message");
                    return;
                }

                Message message = Message.FromJson(e.Data);
                if(message.Type == "AUTHRESPONSE")
                {
                    if(message.Data[0] == "300")
                    {
                        if(message.Data.Length !=2 || message.Data[1].Length != 2048)
                        {
                            Console.WriteLine("\nAuthResponse returned success but no authkey was attached or authkey is malformed.");
                            return;
                        }
                        key = message.Data[1];
                    }
                    Console.WriteLine("\nAuthResponse returned: " + message.Data[0]);
                    return;
                }

                if(message.Type == "MESSAGE")
                {
                    if(message.Data.Length != 3)
                    {
                        Console.WriteLine("Received malformed message");
                        return;
                    }
                    Console.WriteLine($"MESSAGE FROM {message.Data[1]}(USERID:{message.Data[0]}) >> {message.Data[2]}");
                    return;
                }

                if(message.Type == "RESPONSE")
                {
                    Console.WriteLine($"Received response with code: {message.Data[0]} and additional datas:");
                    for(int i = 1; i < message.Data.Length; i++)
                    {
                        Console.WriteLine("---: " + message.Data[i]);
                    }
                    return;
                }

                Console.WriteLine($"Message with type {message.Type} received, contents:");
                foreach(var i in message.Data)
                {
                    Console.WriteLine("---: " + i);
                }
            };

            ws.Connect();

            if(!ws.IsAlive)
            {
                Console.WriteLine("Unable to connect! exiting..");
                return;
            }
            
            PrintCommands();

            while(true)
            {
                Console.Write("> ");
                string input = Console.ReadLine().Trim();
                Console.WriteLine(input);
                if(input.StartsWith("exit")) break;
                if(input.StartsWith("commands"))
                {
                    PrintCommands();
                    continue;
                }
                
                if(input.StartsWith("register"))
                {
                    List<string> str = new List<string>(input.Split(" "));
                    str.RemoveAt(0);
                    if(str.Count != 3)
                    {
                        Console.WriteLine("Usage: register <username> <password> <email>");
                        continue;
                    }
                    ws.Send(new Message("REGISTER", str.ToArray()).ToJson());
                    Console.WriteLine("Registered");
                    continue;
                }
                if(input.StartsWith("login"))
                {
                    List<string> str = new List<string>(input.Split(" "));
                    str.RemoveAt(0);
                    if(str.Count != 2)
                    {
                        Console.WriteLine("Usage: login <username> <password>");
                        continue;
                    }
                    ws.Send(new Message("LOGIN", str.ToArray()).ToJson());
                    Console.WriteLine("Logged in");
                    continue;
                }
                if(key.IsNullOrEmpty())
                {
                    Console.WriteLine("Command not found or not logged in");
                    continue;
                }

                if(input.StartsWith("send"))
                {
                    List<string> str = new List<string>(input.Split(" "));
                    str.RemoveAt(0);
                    if(str.Count < 2)
                    {
                        Console.WriteLine("Usage: send <username> <message>");
                    }
                    string tosend = str[0];
                    str.RemoveAt(0);
                    ws.Send(new Message("SENDTOUSERNAME", new string[] { key, tosend, string.Join("", str)}).ToJson());
                    continue;
                }
                Console.ReadKey();
            }
        }
        private static void PrintCommands() 
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("     exit");
            Console.WriteLine("     register <username> <password> <email>");
            Console.WriteLine("     login <username> <password> ");
            Console.WriteLine("     loggedin");
            Console.WriteLine("     send <username> <message>");
        }
    }
}
