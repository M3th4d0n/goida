using Microsoft.AspNetCore.Mvc;
using goida.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace goida.Controllers
{
    public class AccountController : Controller
    {
        private readonly ChatContext _context;

        public AccountController(ChatContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == user.Nickname);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Nickname", "Пользователь с таким ником уже существует.");
                    return View(user);
                }

                user.Role = "member"; 
                user.RegistrationDate = DateTime.UtcNow; 

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("UserName", user.Nickname);
                return RedirectToAction("Index", "Chat");
            }
            return View(user);
        }


        

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nickname, string password) 
        {
            
            var user = _context.Users.FirstOrDefault(u => u.Nickname == nickname && u.Password == password); 

            if (user != null)
            {
                
                HttpContext.Session.SetString("UserName", user.Nickname);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Chat");
            }

            
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(); 
        }


        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
