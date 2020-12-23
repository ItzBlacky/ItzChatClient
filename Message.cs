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
                Message message = JsonSerializer.Deserialize<Message>(Json);
                return message;
        }
    }
}
