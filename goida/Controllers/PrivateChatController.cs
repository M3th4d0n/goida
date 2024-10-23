using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using goida.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace goida.Controllers
{
    public class PrivateChatController : Controller
    {
        private readonly ChatContext _context;

        public PrivateChatController(ChatContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDm()
        {
            string userName = HttpContext.Session.GetString("UserName");
            var user = _context.Users.FirstOrDefault(u => u.Nickname == userName);
            if (user == null)
            {
                return Unauthorized(); // Если пользователь не найден, возвращаем "Unauthorized"
            }

            string chatCode = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            var privateChat = new PrivateChat
            {
                CreatorId = user.Id,
                ChatCode = chatCode
            };

            _context.PrivateChats.Add(privateChat);
            await _context.SaveChangesAsync();

            // Сохраняем сгенерированную ссылку в TempData
            TempData["ChatLink"] = Url.Action("JoinDm", "PrivateChat", new { code = chatCode }, Request.Scheme);

            // Перенаправляем на страницу с отображением ссылки
            return RedirectToAction("DmCreated");
        }

        [HttpGet]
        public IActionResult DmCreated()
        {
            // Проверяем, есть ли ссылка в TempData
            if (TempData["ChatLink"] == null)
            {
                return RedirectToAction("Index", "Chat"); // Если нет ссылки, возвращаемся в общий чат
            }

            // Передаем ссылку в представление
            ViewBag.ChatLink = TempData["ChatLink"].ToString();
            return View();
        }

        public async Task<IActionResult> JoinDm(string code)
        {
            var chat = await _context.PrivateChats
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.ChatCode == code);

            if (chat == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("PrivateChatId", chat.Id);
            return View("PrivateChatRoom", chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendPrivateMessage(string messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                ModelState.AddModelError("Message", "Message cannot be empty.");
                return RedirectToAction("Index", "Chat");
            }

            int? chatId = HttpContext.Session.GetInt32("PrivateChatId");
            if (!chatId.HasValue)
            {
                return BadRequest("No active chat.");
            }

            string userName = HttpContext.Session.GetString("UserName");
            var user = _context.Users.FirstOrDefault(u => u.Nickname == userName);
            if (user == null)
            {
                return Unauthorized();
            }

            var privateMessage = new PrivateMessage
            {
                ChatId = chatId.Value,
                UserId = user.Id,
                Content = messageContent
            };

            _context.PrivateMessages.Add(privateMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("JoinDm", new { code = _context.PrivateChats.Find(chatId.Value)?.ChatCode });
        }
    }
}
