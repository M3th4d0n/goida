using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using goida.Models;


namespace goida.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatContext _context;

        public ChatController(ChatContext context)
        {
            _context = context;
        }


        private static List<string> messages = new List<string>();

        public IActionResult Index()
        {
	        string userName = HttpContext.Session.GetString("UserName");
	        if (string.IsNullOrEmpty(userName))
	        {
		        // Замените "SetUserName" на "Register", "Account" укажите имя вашего контроллера регистрации
		        return RedirectToAction("Register", "Account");
	        }

	        // Получаем все сообщения из базы данных и сортируем их по времени
	        var messages = _context.Messages.OrderByDescending(m => m.Time).ToList();
	        return View(messages); // Передаем сообщения в представление
        }




		[HttpPost]
		public IActionResult SendMessage(string message)
		{
			string userName = HttpContext.Session.GetString("UserName");

			if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(userName))
			{
				var newMessage = new Message
				{
					UserName = userName,
					Content = message,
					Time = DateTime.Now // Устанавливаем текущее время
				};

				_context.Messages.Add(newMessage); // Добавляем новое сообщение в контекст
				_context.SaveChanges(); // Сохраняем изменения в базе данных
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
        public IActionResult SetUserName()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveUserName(string name)
        {
            if(!string.IsNullOrEmpty(name))
            {
                HttpContext.Session.SetString("UserName", name);
                return RedirectToAction("Index");
            }
            return View("SetUserName");
        }
    }
}
