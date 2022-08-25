namespace ReenbitChat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
