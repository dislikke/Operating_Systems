

namespace ChatApp.Models
{
    public class Message
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public Message(string content)
        {
            Content = content;
            Timestamp = DateTime.Now;
        }
    }
}









