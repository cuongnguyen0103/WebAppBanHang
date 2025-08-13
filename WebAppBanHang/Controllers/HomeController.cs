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

        //VIEW INDEX
        public IActionResult Index()
        {
            //lay session nhan tu Login
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;

            return View();
        }

        // NEW PRODUCT
        public IActionResult NewProduct()
        {
            var products = _context.Products.ToList();

            return Ok(products);
        }

        #region LOGIN/LOGOUT/USERINFO
        //LOGIN
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (user.Password != password)
            {
                return BadRequest("Incorrect password.");
            }
            // lay session
            HttpContext.Session.SetString("UserName", user.UserName);
            //return View("Index");
            return RedirectToAction("Index", "Home");
        }

        //LOGOUT
        public IActionResult Logout()
        {
            // delete session
            HttpContext.Session.Remove("UserName");
            // tra ve trang Login
            return RedirectToAction("Index", "Home");
        }

        // User information
       
        public IActionResult UserInfo()
        {
            //lay session nhan tu Login
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName; //thong bao cho View
            //lay userId tu session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }            
            var user = _context.Users.Find(userName);
            if (user == null)
            {
                return NotFound("Ko thay thong tin nguoi dung");
            }
            else if (user.UserName == "admin")
            {
                return RedirectToAction("AdminView");
            }
            else if (user.UserName != "admin")
            {
                return RedirectToAction("Customer");
            }
            //return View(user);
        }
        #endregion
        #region VIEW CUSTOMER
        public IActionResult Customer()
        {
            return View();
        }

        //VIEW Admin

        public IActionResult AdminView()
        {            
            return View();
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
            return Ok();
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
        //GET products
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            if (products == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(products);
        }

        // POST Product Admin
        [HttpPost]
        public async Task<IActionResult> AddProductAdmin(Product input)
        {
            Product product = new Product();
            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.StockQuantity = input.StockQuantity;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.IsActive = input.IsActive;

            _context.Products.Add(product);
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

        // Delete Product Admin
        [HttpDelete("/Home/DeleteProductAdmin/{id}")]
        public async Task<IActionResult> DeleteProductAdmin(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound("product not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET id to UpdateProduct
        [HttpGet("/Home/DetailProductAdmin/{id}")]
        public async Task<IActionResult> DetailProductAdmin(int id)
        {
            //lay du lieu sql
            var item = _context.Products.FirstOrDefault(x => x.ProductId == id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        // PUT Product Id
        [HttpPut("/Home/UpdateProductAdmin/{id}")]
        public async Task<IActionResult> UpdateProductAdmin(int id, Product input)
        {
            //Data Validation 
            if (!ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return View(input);
            }
            var update = _context.Products.Find(id);
            if (update == null)
            {
                return NotFound();
            }
            else
            {
                update.Name = input.Name;
                update.Description = input.Description;
                update.Price = input.Price;
                update.StockQuantity = input.StockQuantity;
                update.UpdatedAt = DateTime.Now;
                update.IsActive = input.IsActive;

                await _context.SaveChangesAsync();
            }
            return Ok();
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
