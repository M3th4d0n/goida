using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using goida.Models;
using Microsoft.EntityFrameworkCore;



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
		        
		        return RedirectToAction("Register", "Account");
	        }

	        
	        var messages = _context.Messages.OrderByDescending(m => m.Time).ToList();
	        return View(messages);
        }


        private int GetUserId()
        {
	        
	        string userName = HttpContext.Session.GetString("UserName");
    
	        
	        var user = _context.Users.FirstOrDefault(u => u.Nickname == userName);
    
	        
	        return user?.Id ?? 0; 
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(string messageContent)
        {
	       
	        if (string.IsNullOrWhiteSpace(messageContent))
	        {
		        ModelState.AddModelError("Message", "Сообщение не может быть пустым.");
		        return RedirectToAction("Index"); 
	        }

	        var userId = GetUserId(); 
	        var roomId = HttpContext.Session.GetInt32("RoomId");

	        var message = new Message
	        {
		        UserName = HttpContext.Session.GetString("UserName"),
		        Content = messageContent,
		        Time = DateTime.Now,
		        UserId = userId,
	        };

	        await _context.Messages.AddAsync(message);
	        await _context.SaveChangesAsync(); 

	        if (roomId.HasValue)
	        {
		        return RedirectToAction("ChatRoom", new { id = roomId.Value });
	        }

	        return RedirectToAction("Index", "Chat");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
	        
	        if (HttpContext.Session.GetString("UserRole") != "admin")
	        {
		        return Forbid();  
	        }

	        
	        var message = await _context.Messages.FindAsync(id);
	        if (message == null)
	        {
		        return NotFound();  
	        }

	        // Удаляем сообщение
	        _context.Messages.Remove(message);
	        await _context.SaveChangesAsync();

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
