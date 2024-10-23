using System;

namespace goida.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;

        public virtual PrivateChat Chat { get; set; }
        public virtual User User { get; set; }
    }
}