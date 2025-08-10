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

        //GET USER Admin
        public IActionResult GetUser()
        {
            var users = _context.Users.ToList();
            if (users == null)
            {
                return NotFound("User not found.");
            }
            return Ok(users);
        }
        // View AdminView
        public IActionResult AdminView()
        {            
            return View();
        }

        // POST USER Admin
        [HttpPost]
        public async Task<IActionResult> AddUserAdmin(User input)
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
            user.IsActive = input.IsActive;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction();
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

        // Delete User Admin
        [HttpDelete("/Home/DeleteUserAdmin/{id}")]
        public async Task<IActionResult> DeleteUserAdmin(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET id to UpdateUser
        [HttpGet("/Home/DetailUserAdmin/{id}")]
        public async Task<IActionResult> DetailUserAdmin(int id)
        {
            //lay du lieu sql
            var item = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        // PUT User Id
        [HttpPut("/Home/UpdateUserAdmin/{id}")]
        public async Task<IActionResult> UpdateUserAdmin(int id, User input)
        {
            //Data Validation 
            if (!ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return View(input);
            }
            var update = _context.Users.Find(id);
            if (update == null)
            {
                return NotFound();
            }
            else
            {
                update.UserName = input.UserName;
                update.Password = input.Password;
                update.FullName = input.FullName;
                update.DateOfBirth = input.DateOfBirth;
                update.Email = input.Email;
                update.PhoneNumber = input.PhoneNumber;
                update.UpdatedAt = DateTime.Now;
                update.IsActive = input.IsActive;

                await _context.SaveChangesAsync();
            }
            return Ok();
        }


        // PRODUCTS
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
