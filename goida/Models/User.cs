using System.ComponentModel.DataAnnotations;

namespace goida.Models
{
    public class User
    {
        public int Id { get; set; } // Уникальный идентификатор пользователя

        [Required(ErrorMessage = "Nickname is required.")]
        public string Nickname { get; set; } // Ник пользователя

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Обычный пароль
    }
}