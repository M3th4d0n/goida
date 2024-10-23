using System;

namespace goida.Models
{
    public class Message
    {
        public int Id { get; set; } // Уникальный идентификатор сообщения
        public string UserName { get; set; } // Имя пользователя, отправившего сообщение
        public string Content { get; set; } // Содержимое сообщения
        public DateTime Time { get; set; } // Время отправки сообщения
        public int UserId { get; set; } // Уникальный идентификатор пользователя (добавлено)
    }
}