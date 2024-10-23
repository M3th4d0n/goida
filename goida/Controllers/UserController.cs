using Microsoft.AspNetCore.Mvc;
using goida.Models;
using Microsoft.EntityFrameworkCore;

namespace goida.Controllers
{
    public class UserController : Controller
    {
        private readonly ChatContext _context;

        public UserController(ChatContext context)
        {
            _context = context;
        }

        
        [HttpGet("user/{id}")]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(); // Если пользователь не найден
            }

            return View(user);
        }
    }
}