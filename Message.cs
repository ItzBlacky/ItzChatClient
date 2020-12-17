using System;
using System.Text.Json;

namespace ItzChatClient
{
    public class Message
    {
        public string Type { get; set; }
        public string[] Data { get; set; }

        public Message(string Type, string[] Data)
        {
            this.Type = Type;
            this.Data = Data;
        }
        public Message()
        {
            
        }
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message FromJson(string Json)
        {
                Console.WriteLine($"Trying to convert json: {Json}");
                Message message = JsonSerializer.Deserialize<Message>(Json.Replace("\\u002", ""));
                Console.WriteLine($"1");
                return message;
        }
    }
}
