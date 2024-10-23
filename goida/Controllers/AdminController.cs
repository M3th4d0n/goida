using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using goida.Models;
using System.Linq;
using System.Threading.Tasks;

namespace goida.Controllers
{
    public class AdminController : Controller
    {
        private readonly ChatContext _context;

        public AdminController(ChatContext context)
        {
            _context = context;
        }

        public IActionResult UserList()
        {
            // Проверяем, что роль пользователя - admin
            if (HttpContext.Session.GetString("UserRole") != "admin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Role = newRole; 
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> UserProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}