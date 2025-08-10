using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebAppBanHang.Models;
using WebAppBanHang.Models.Entity;

namespace WebAppBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebAppBanHangContext _context;

        public HomeController(ILogger<HomeController> logger,WebAppBanHangContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //LOGIN
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == u.Password);
            if (user == null)
            {
                return NotFound("UserName or Password error.");
            }
            else
            {
                return View("Index");
            }
        }

        //GET USER
        public IActionResult GetUser()
        {
            var users = _context.Users.ToList();
            if (users == null)
            {
                return NotFound("User not found.");
            }
            return Ok(users);
        }
        //AdminView
        public IActionResult AdminView()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminView(User input)
        {
            User user = new User();
            user.UserName = input.UserName;
            user.Password = input.Password;
            user.FullName = input.FullName;
            user.DateOfBirth = input.DateOfBirth;
            user.Email = input.Email;
            user.PhoneNumber = input.PhoneNumber;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return View();
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login");
            //}
            //var user = _context.Users.Find(userId);
            //if (user == null)
            //{
            //    return NotFound("Ko thay thong tin nguoi dung");
            //}
            //return View(user);
            }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
