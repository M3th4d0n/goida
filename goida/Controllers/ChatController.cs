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


        private int GetUserId()
        {
	        // Получаем имя пользователя из сессии
	        string userName = HttpContext.Session.GetString("UserName");
    
	        // Ищем пользователя в базе данных по имени
	        var user = _context.Users.FirstOrDefault(u => u.Nickname == userName);
    
	        // Если нашли, возвращаем его ID, иначе 0 или выбрасываем исключение
	        return user?.Id ?? 0; // Здесь 0 — это условное значение, можно выбрасывать исключение или возвращать null
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(string messageContent)
        {
	        // Проверка на null или пустое содержимое
	        if (string.IsNullOrWhiteSpace(messageContent))
	        {
		        // Обработка ошибки: сообщение не должно быть пустым
		        ModelState.AddModelError("Message", "Сообщение не может быть пустым.");
		        return RedirectToAction("Index"); // Вернуться на страницу чата
	        }

	        var userId = GetUserId(); // Получение идентификатора пользователя
	        var message = new Message
	        {
		        UserName = HttpContext.Session.GetString("UserName"),
		        Content = messageContent,
		        Time = DateTime.Now,
		        UserId = userId // Устанавливаем идентификатор пользователя
	        };

	        await _context.Messages.AddAsync(message);
	        await _context.SaveChangesAsync(); // Сохранение изменений в базе данных

	        return RedirectToAction("Index", "Chat");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
	        // Проверяем, является ли пользователь администратором
	        if (HttpContext.Session.GetString("UserRole") != "admin")
	        {
		        return Forbid();  // Если роль не admin, доступ запрещен
	        }

	        // Ищем сообщение по ID
	        var message = await _context.Messages.FindAsync(id);
	        if (message == null)
	        {
		        return NotFound();  // Сообщение не найдено
	        }

	        // Удаляем сообщение
	        _context.Messages.Remove(message);
	        await _context.SaveChangesAsync();  // Сохраняем изменения

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
