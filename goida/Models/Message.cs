using System;

namespace goida.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
        public int UserId { get; set; }

    }

}