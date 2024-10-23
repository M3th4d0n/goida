using System;
using System.Collections.Generic;

namespace goida.Models
{
    public class PrivateChat
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string ChatCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<PrivateMessage> Messages { get; set; }
        public virtual User Creator { get; set; }
    }
}