using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebAppBanHang.Models;
using WebAppBanHang.Models.Entity;
using WebAppBanHang.Models.Entity.Dto;
using WebAppBanHang.ViewModels;


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
        #region VIEW INDEX
        public IActionResult Index()
        {
            //lay session nhan tu Login
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;

            return View();
        }
        // Search Product
        [HttpGet("Home/SearchProduct/{name}")]
        public IActionResult SearchProduct(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Tên sản phẩm không được để trống.");
            }
            var products = _context.Products
                .Where(p => p.Name.Contains(name) && p.IsActive)
                .Include(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    DiscountPercent = p.ProductDiscounts
                        .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                    pd.Discount.StartDate <= DateTime.Now &&
                                    pd.Discount.EndDate >= DateTime.Now)
                        .Select(pd => (decimal?)pd.Discount.DiscountPercent)
                        .FirstOrDefault() ?? 0 // lấy giảm giá đầu tiên nếu có, mặc định là 0
                })
                .ToList();
            if (!products.Any())
            {
                Ok(new List<object>()); // Trả về mảng rỗng
            }
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
                return BadRequest("Incorrect UserName.");
            }
            if (user.Password != password)
            {
                return BadRequest("Incorrect password.");
            }
            // lay session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Role", user.Role);
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
        // Sign Up
        [HttpPost]
        public async Task<IActionResult> Signup(User input)
        {
            //string confirmPassword = Request.Form["confirmpassword"];
            // Data Validation
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            // Kiem tra username-error
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == input.UserName);
            if (existingUser != null)
            {
                return Json(new { success = false, usernameError = "Tên đăng nhập đã tồn tại." });
            }
            // get data from input
            User user = new User();
            user.UserName = input.UserName;
            user.Password = input.Password;
            user.ConfirmPassword = input.ConfirmPassword;
            //user.Role = string.IsNullOrWhiteSpace(input.Role) ? "customer" : input.Role;
            user.Role = "admin";
            user.Email = input.Email;
            user.IsActive = input.IsActive ? input.IsActive : true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // lay session
            //HttpContext.Session.SetString("UserName", user.UserName);
            //HttpContext.Session.SetString("Role", user.Role);
            //return View("Index");
            //return RedirectToAction("Index", "Home");
            // lay session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Role", user.Role);
            return View("Index");
        }

        // User information

        public IActionResult UserInfo()
        {
            //lay session nhan tu Login
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Role = role; //thong bao cho View
            //lay userId tu session
            //var userId = HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login");
            //}            
            //var user = _context.Users.Find(userName);
            //if (user == null)
            //{
            //    return NotFound("Ko thay thong tin nguoi dung");
            //}
            if (role == "admin")
            {
                return RedirectToAction("AdminView");
            }
            else
            {
                return RedirectToAction("CustomerView");
            }
            //return View(user);
        }
        #endregion
        // NEW PRODUCT (san pham moi trong vong 15 ngay tu ngay tao)
        #region NEW PRODUCT
        // GET NEW PRODUCT 
        //public IActionResult NewProduct()
        //{
        //    var products = _context.Products
        //                        .Include(p => p.ProductDiscounts)
        //                        .ThenInclude(pd => pd.Discount)
        //                        .Select(p => new
        //                        {
        //                            p.ProductId,
        //                            p.Name,
        //                            p.Description,
        //                            p.Price,
        //                            p.StockQuantity,
        //                            DiscountPercent = p.ProductDiscounts
        //                            .Where(pd => pd.IsActive && pd.Discount.IsActive &&
        //                                        pd.Discount.StartDate <= DateTime.Now &&
        //                                        pd.Discount.EndDate >= DateTime.Now)
        //                            .Select(pd => pd.Discount.DiscountPercent)
        //                        .FirstOrDefault() // lấy giảm giá đầu tiên nếu có, mặc định là 0
        //                        })
        //                        .ToList();
        //    return Ok(products);
        //}
        [HttpGet("Home/NewProducts")]
        public IActionResult NewProducts()
        {
            //lay session nhan tu Login
            //var userName = HttpContext.Session.GetString("UserName");
            //ViewBag.UserName = userName;
            // Lay danh sach san pham moi trong vong 15 ngay
            var products = _context.Products
                .Where(p => p.CreatedAt >= DateTime.Now.AddDays(-15) && p.IsActive)
                .Include(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    DiscountPercent = p.ProductDiscounts
                        .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                    pd.Discount.StartDate <= DateTime.Now &&
                                    pd.Discount.EndDate >= DateTime.Now)
                        .Select(pd => (decimal?)pd.Discount.DiscountPercent)
                        .FirstOrDefault() ?? 0 // lấy giảm giá đầu tiên nếu có, mặc định là 0
                })
                .ToList();
            return Ok(products);
        }        

        #endregion NEW PRODUCT

        // Promotional Products (san pham co giam gia và lớn hơn 0)
        #region PROMOTIONAL PRODUCTS
        [HttpGet("Home/PromotionalProducts")]
        public IActionResult PromotionalProducts()
        {
            //lay session nhan tu Login
            //var userName = HttpContext.Session.GetString("UserName");
            //ViewBag.UserName = userName;
            // Lay danh sach san pham co giam gia
            var products = _context.Products
                .Where(p => p.ProductDiscounts.Any(pd => pd.IsActive && pd.Discount.IsActive &&
                                                        pd.Discount.StartDate <= DateTime.Now &&
                                                        pd.Discount.EndDate >= DateTime.Now) && p.IsActive)
                .Include(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    DiscountPercent = p.ProductDiscounts
                        .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                    pd.Discount.StartDate <= DateTime.Now &&
                                    pd.Discount.EndDate >= DateTime.Now)
                        .Select(pd => (decimal?)pd.Discount.DiscountPercent)
                        .FirstOrDefault() ?? 0 // lấy giảm giá đầu tiên nếu có, mặc định là 0
                })
                .ToList();
            return Ok(products);
        }

        #endregion PROMOTIONAL PRODUCTS

        // GET ALL PRODUCTS
        #region GET ALL PRODUCTS
        [HttpGet("Home/AllProducts")]
        public IActionResult AllProducts()
        {
            //lay session nhan tu Login
            //var userName = HttpContext.Session.GetString("UserName");
            //ViewBag.UserName = userName;
            // Lay danh sach tat ca san pham
            var products = _context.Products
                .Where(p => p.IsActive) // chi lay san pham active
                .Include(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    DiscountPercent = p.ProductDiscounts
                        .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                    pd.Discount.StartDate <= DateTime.Now &&
                                    pd.Discount.EndDate >= DateTime.Now)
                        .Select(pd => (decimal?)pd.Discount.DiscountPercent)
                        .FirstOrDefault() ?? 0 // lấy giảm giá đầu tiên nếu có, mặc định là 0
                })
                .ToList();
            return Ok(products);
        }
        #endregion GET ALL PRODUCTS
        // BUY PRODUCT
        //xac thuc người dùng đã đăng nhập và mua sản phẩm
        #region BUY PRODUCT
        [HttpPost("Home/BuyProduct")]
        public IActionResult BuyProduct(ProductDto input)
        {
            //lay session nhan tu Login
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return Json(new { success = false, requireLogin = true });
            }
            else
            {
                // kiem tra so luong ton kho
                var product_sl = _context.Products.Find(input.ProductId);
                if (product_sl == null || product_sl.StockQuantity < input.Quantity)
                {
                    return BadRequest("Sản phẩm không đủ số lượng trong kho.");
                }
                // Kiem tra xem UserId va Status cua Order
                var existingOrder = _context.Orders
                    .Where(o => o.UserId == userId && o.Status == "Pending" && o.IsActive)
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault();

                // tim discountId
                var discountId = _context.ProductDiscounts
                    .Where(pd => pd.ProductId == input.ProductId && pd.IsActive)
                    .Join(_context.Discounts,
                        pd => pd.DiscountId,
                        d => d.DiscountId,
                        (pd, d) => new { pd, d })
                    .Where(x => x.d.DiscountPercent == input.DiscountPercent)
                    .Select(x => x.d.DiscountId)
                    .FirstOrDefault();

                // Create new OrderDetail
                var orderDetail = new OrderDetail
                {
                    ProductId = input.ProductId,
                    Quantity = input.Quantity,
                    Price = input.Price,
                    DiscountId = discountId != 0 ? discountId : null, // nếu không tìm thấy thì để null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsActive = true
                };
                // Them hoac cap nhat Order
                if (existingOrder != null)
                {
                    // Cập nhật OrderDetails trong đơn hàng hiện tại
                    //kiem tra xem OrderDetail da ton tai chua
                    var existingOrderDetail = existingOrder.OrderDetails
                        .FirstOrDefault(od => od.ProductId == input.ProductId && od.IsActive);
                    if (existingOrderDetail != null)
                    {
                        // Cập nhật số lượng và giá của OrderDetail hiện tại
                        existingOrderDetail.Quantity += input.Quantity;
                        existingOrderDetail.Price = input.Price;
                        existingOrderDetail.DiscountId = discountId != 0 ? discountId : null; // nếu không tìm thấy thì để null
                    }
                    else
                    {
                        // Thêm OrderDetail mới vào đơn hàng hiện tại
                        existingOrder.OrderDetails.Add(orderDetail);
                    }
                    // tinh lai tong tien
                    decimal total = 0;
                    foreach (var detail in existingOrder.OrderDetails)
                    {
                        //total += detail.Quantity * input.Price * (1 - (input.DiscountPercent / 100m));
                        var product = _context.Products.Find(detail.ProductId);//có thể bỏ
                        if (product != null)
                        {
                            var discountPercent = _context.ProductDiscounts
                                .Where(pd => pd.ProductId == detail.ProductId && pd.IsActive)
                                .Join(_context.Discounts,
                                    pd => pd.DiscountId,
                                    d => d.DiscountId,
                                    (pd, d) => new { pd, d })
                                .Select(x => (decimal?)x.d.DiscountPercent) // ép kiểu về decimal
                                .FirstOrDefault() ?? 0m;
                            total += detail.Quantity * product.Price * (1 - (discountPercent / 100m));
                        }
                    }
                    //them tong tien vao Order
                    existingOrder.TotalAmount = total;
                    _context.SaveChanges();
                    return Ok("Cập nhật đơn hàng thành công!");
                }
                else
                {
                    // Create new Order
                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        CreatedAt = DateTime.Now,
                        //TotalAmount = 0, // Sẽ cập nhật sau khi tính toán
                        IsActive = true,
                        OrderDetails = new List<OrderDetail>()
                    };
                    // total amount
                    decimal total = input.Quantity * input.Price * (1 - (input.DiscountPercent / 100m));
                    //decimal total = 0;
                    var product = _context.Products.Find(input.ProductId);
                    if (product != null)
                    {
                        var discountPercent = _context.ProductDiscounts
                            .Where(pd => pd.ProductId == input.ProductId && pd.IsActive)
                            .Join(_context.Discounts,
                                pd => pd.DiscountId,
                                d => d.DiscountId,
                                (pd, d) => new { pd, d })
                            .Select(x => (decimal?)x.d.DiscountPercent) //ep kiểu về decimal
                            .FirstOrDefault() ?? 0m;
                    }
                    // Cập nhật tổng tiền trong đơn hàng
                    order.TotalAmount = total;
                    // Add OrderDetail to Order
                    order.OrderDetails.Add(orderDetail);

                    //// cap nhat so luong ton kho
                    //product_sl.StockQuantity -= input.Quantity;
                    //if (product_sl.StockQuantity == 0)
                    //{
                    //    // Nếu số lượng tồn kho bằng 0, đánh dấu sản phẩm là không hoạt động
                    //    product_sl.IsActive = false;
                    //}
                    //_context.Products.Update(product_sl);

                    // save
                    _context.Orders.Add(order);
                    _context.SaveChanges();
                    // Trả về thông báo thành công
                    return Ok("Mua hàng thành công!");
                }

            }
        }
        // GET DATA PRODUCT BY ID
        [HttpGet("Home/GetProductById/{productid}")]
        public async Task<IActionResult> GetProductById(int productid)
        {
            var product = _context.Products
                                .Include(p => p.ProductDiscounts)
                                .ThenInclude(pd => pd.Discount)
                                .Where(p => p.ProductId == productid)
                                .Select(p => new ProductDto                 // DTO
                                {
                                    ProductId = p.ProductId,
                                    Name = p.Name,
                                    Description = p.Description,
                                    Price = p.Price,
                                    StockQuantity = p.StockQuantity,
                                    DiscountPercent = p.ProductDiscounts
                                    .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                                pd.Discount.StartDate <= DateTime.Now &&
                                                pd.Discount.EndDate >= DateTime.Now)
                                    .Select(pd => pd.Discount.DiscountPercent)
                                .FirstOrDefault() // lấy giảm giá đầu tiên nếu có, mặc định là 0
                                })
                                .ToList();
            if (product == null)
            {
                return BadRequest("Sản phẩm không tồn.");
            }
            return Ok(product);
        }
        #endregion BUY PRODUCT

        #endregion
       

        //VIEW CUSTOMER
        #region VIEW CUSTOMER
        public IActionResult CustomerView()
        {
            //lay userId tu session
            var userId = HttpContext.Session.GetInt32("UserId");

            var products = _context.Users.Find(userId);
            if (products == null)
            {
                return NotFound("Product not found.");
            }
            return View(products);
        }
        // GET USER Customer
        [HttpGet]
        public IActionResult GetUserCustomer()
        {
            //lay userId tu session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }
        // get detail User customer
        [HttpGet("/Home/DetailUserCustomer/{id}")]
        public async Task<IActionResult> DetailUserCustomer(int id)
        {
            //lay du lieu sql
            var item = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        // update User
        [HttpPost]
        public async Task<IActionResult> UpdateUserCustomer(User input)
        {
            //Data Validation
            ModelState.Remove(nameof(input.ConfirmPassword));
            if (!ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return View(input);
            }
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var update = _context.Users.Find(userId);
            if (update == null)
            {
                return NotFound();
            }
            else
            {
                update.UserName = input.UserName;
                update.Password = input.Password;
                update.FullName = input.FullName;
                update.Role = string.IsNullOrWhiteSpace(input.Role) ? "customer" : input.Role;
                update.DateOfBirth = input.DateOfBirth;
                update.Email = input.Email;
                update.PhoneNumber = input.PhoneNumber;
                update.UpdatedAt = DateTime.Now;
                update.IsActive = input.IsActive ? input.IsActive : true;
                await _context.SaveChangesAsync();
                return RedirectToAction("CustomerView");
            }
        }
        #endregion  VIEW CUSTOMER

        //VIEW Admin
        #region VIEW Admin
        public IActionResult AdminView()
        {
            var model = new AllTablesViewModel
            {
                Products = _context.Products.ToList(),
                Users = _context.Users.ToList(),
                Orders = _context.Orders.ToList(),
                OrderDetails = _context.OrderDetails.ToList(),
                Discounts = _context.Discounts.ToList(),
                ProductDiscounts = _context.ProductDiscounts.ToList()
            };

            return View(model);
            //return View();
        }
        //Table User
        #region Table User
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
            //Data Validation
            ModelState.Remove(nameof(input.ConfirmPassword));
            ModelState.Remove(nameof(input.IsActive));
            if (input==null || !ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return View(input);
            }
            // Kiem tra UserName da ton tai
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == input.UserName);
            if (existingUser != null)
            {
                return NotFound("User đã tồn tại!!");
            }
            else
            {
                User user = new User();
                user.UserName = input.UserName;
                user.Password = input.Password;
                user.Role = string.IsNullOrWhiteSpace(input.Role) ? "customer" : input.Role;
                user.FullName = input.FullName;
                user.DateOfBirth = input.DateOfBirth;
                user.Email = input.Email;
                user.PhoneNumber = input.PhoneNumber;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.IsActive = input.IsActive ? input.IsActive : true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
                      
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
            // Remove ConfirmPassword from ModelState to avoid validation errors
            ModelState.Remove(nameof(input.ConfirmPassword));
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
                update.Role = string.IsNullOrWhiteSpace(input.Role) ? "customer" : input.Role;
                update.DateOfBirth = input.DateOfBirth;
                update.Email = input.Email;
                update.PhoneNumber = input.PhoneNumber;
                update.UpdatedAt = DateTime.Now;
                update.IsActive = input.IsActive ? input.IsActive : true;

                await _context.SaveChangesAsync();
                return Ok();
            }            
        }
        #endregion Table User

        // Table Product
        #region Table Product        
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
            //data Validation
            if (input == null || !ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return Ok();
            }
            Product product = new Product();
            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.StockQuantity = input.StockQuantity;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
            product.IsActive = input.IsActive ? input.IsActive : true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction();            
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
                update.IsActive = input.IsActive ? input.IsActive : true;

                await _context.SaveChangesAsync();
                return Ok();
            }
        }
        #endregion Table Product

        // Table Order
        #region Table Order
        //GET orders
        public IActionResult GetOrders()
        {
            var orders = _context.Orders.ToList();

            if (orders == null)
            {
                return NotFound("Order not found.");
            }
            return Ok(orders);
        }

        // Delete Order Admin
        [HttpDelete("/Home/DeleteOrderAdmin/{id}")]
        public async Task<IActionResult> DeleteOrderAdmin(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok();
        }
        #endregion Table Order

        // Table OrderDetail
        #region Table OrderDetail
        //GET orderdetails
        public IActionResult GetOrderDetails()
        {
            var orderDetails = _context.OrderDetails.ToList();
            if (orderDetails == null)
            {
                return NotFound("OrderDetail not found.");
            }
            return Ok(orderDetails);
        }

        #endregion Table OrderDetail
        // Table Discount
        #region Table Discount
        //GET discounts
        public IActionResult GetDiscounts()
        {
            var discounts = _context.Discounts.ToList();
            if (discounts == null)
            {
                return NotFound("Discount not found.");
            }
            return Ok(discounts);
        }

        // POST Discount Admin
        [HttpPost]
        public async Task<IActionResult> AddDiscountAdmin(Discount input)
        {
            if (input == null || !ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return Ok();
            }
            Discount discount = new Discount();
            discount.Code = input.Code;
            discount.DiscountDescription = input.DiscountDescription;
            discount.DiscountPercent = input.DiscountPercent;
            discount.StartDate = input.StartDate;
            discount.EndDate = input.EndDate;
            discount.CreatedAt = DateTime.Now;
            discount.UpdatedAt = DateTime.Now;
            discount.IsActive = input.IsActive ? input.IsActive : true;
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // Delete Discount Admin
        [HttpDelete("/Home/DeleteDiscountAdmin/{id}")]
        public async Task<IActionResult> DeleteDiscountAdmin(int id)
        {
            var discount = _context.Discounts.Find(id);
            if (discount == null)
            {
                return NotFound("Discount not found.");
            }
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // GET id to UpdateDiscount
        [HttpGet("/Home/DetailDiscountAdmin/{id}")]
        public async Task<IActionResult> DetailDiscountAdmin(int id)
        {
            //lay du lieu sql
            var item = _context.Discounts.FirstOrDefault(x => x.DiscountId == id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        // PUT Discount Id
        [HttpPut("/Home/UpdateDiscountAdmin/{id}")]
        public async Task<IActionResult> UpdateDiscountAdmin(int id, Discount input)
        {
            //Data Validation 
            if (!ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return View(input);
            }
            var update = _context.Discounts.Find(id);
            if (update == null)
            {
                return NotFound();
            }
            else
            {
                update.Code = input.Code;
                update.DiscountDescription = input.DiscountDescription;
                update.DiscountPercent = input.DiscountPercent;
                update.StartDate = input.StartDate;
                update.EndDate = input.EndDate;
                update.UpdatedAt = DateTime.Now;
                update.IsActive = input.IsActive ? input.IsActive : true;
                await _context.SaveChangesAsync();
                return Ok();
            }
        }
        #endregion Table Discount

        // table ProductDiscount
        #region Table ProductDiscount
        //GET ProductDiscounts
        public IActionResult GetProductDiscounts()
        {
            var productDiscounts = _context.ProductDiscounts.ToList();
            if (productDiscounts == null)
            {
                return NotFound("ProductDiscount not found.");
            }
            return Ok(productDiscounts);
        }

        // POST ProductDiscount Admin
        [HttpPost]
        public async Task<IActionResult> AddProductDiscountAdmin(ProductDiscount input)
        {
            //Data Validation
            ModelState.Remove(nameof(input.Product)); // bo bien quan he voi Product
            ModelState.Remove(nameof(input.Discount));// bo bien quan he voi Discount
            if (input == null || !ModelState.IsValid)
            {
                // Returns a View with data errors to display information
                return Ok();
            }
            ProductDiscount productDiscount = new ProductDiscount();
            productDiscount.ProductId = input.ProductId;
            productDiscount.DiscountId = input.DiscountId;
            productDiscount.CreatedAt = DateTime.Now;
            productDiscount.IsActive = input.IsActive ? input.IsActive : true;

            _context.ProductDiscounts.Add(productDiscount);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion Table ProductDiscount
        #endregion VIEW Admin

        //View ShoppingCart
        #region VIEW ShoppingCart
        [HttpGet]
        //[Route("Home/ShoppingCart")]
       
        public IActionResult ShoppingCart()
        {
            //lay session nhan tu Login
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;

            //
            var model = new AllTablesViewModel
            {
                Products = _context.Products.ToList(),
                Users = _context.Users.ToList(),
                Orders = _context.Orders.ToList(),
                OrderDetails = _context.OrderDetails.ToList(),
                Discounts = _context.Discounts.ToList(),
                ProductDiscounts = _context.ProductDiscounts.ToList()
            };

            return View(model);
        }
        // get shopping cart
        [HttpGet("Home/GetShoppingCart")]
        public IActionResult GetShoppingCart()
        {
            //lay session nhan tu Login
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return BadRequest("Bạn cần đăng nhập để xem giỏ hàng.");
            }
            var order = _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Pending" && o.IsActive)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    o.OrderDate,
                    o.Status,
                    o.CreatedAt,
                    o.TotalAmount,
                    o.IsActive,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.OrderDetailId,
                        od.ProductId,
                        od.Quantity,
                        od.Price,
                        od.DiscountId,
                        Product = new
                        {
                            od.Product.Name,
                            od.Product.Description,
                            od.Product.Price,
                            od.Product.StockQuantity,
                            DiscountPercent = od.Product.ProductDiscounts
                                .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                             pd.Discount.StartDate <= DateTime.Now &&
                                             pd.Discount.EndDate >= DateTime.Now)
                                .Select(pd => pd.Discount.DiscountPercent)
                                .FirstOrDefault()// lấy giảm giá đầu tiên nếu có, mặc định là 0
                        }
                    })
                })
                .FirstOrDefault();
            if (order == null || !order.OrderDetails.Any())
            {
                //return NotFound("Giỏ hàng trống.");
                return Ok(new List<object>()); // Trả về mảng rỗng
            }
            return Ok(order);//tra về danh sách chi tiết đơn hàng trong giỏ hàng return Ok(order.OrderDetails)
        }
        // get Order Completed
        [HttpGet("Home/GetOrderCompleted")]
        public IActionResult GetOrderCompleted()
        {
            //lay session nhan tu Login
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return BadRequest("Bạn cần đăng nhập để xem giỏ hàng.");
            }
            var order = _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Finished" && o.IsActive)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.ProductDiscounts)
                .ThenInclude(pd => pd.Discount)
                .Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    o.OrderDate,
                    o.Status,
                    o.CreatedAt,
                    o.TotalAmount,
                    o.IsActive,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.OrderDetailId,
                        od.ProductId,
                        od.Quantity,
                        od.Price,
                        od.DiscountId,
                        Product = new
                        {
                            od.Product.Name,
                            od.Product.Description,
                            od.Product.Price,
                            od.Product.StockQuantity,
                            DiscountPercent = od.Product.ProductDiscounts
                                .Where(pd => pd.IsActive && pd.Discount.IsActive &&
                                             pd.Discount.StartDate <= DateTime.Now &&
                                             pd.Discount.EndDate >= DateTime.Now)
                                .Select(pd => pd.Discount.DiscountPercent)
                                .FirstOrDefault() // lấy giảm giá đầu tiên nếu có, mặc định là 0
                        }
                    })
                })
                .FirstOrDefault();
            if (order == null || !order.OrderDetails.Any())
            {
                return Ok(new List<object>()); // Trả về mảng rỗng
            }
            return Ok(order);
        }

        // Pay Order
        [HttpPut("Home/PayOrder/{id}")]
        public IActionResult PayOrder(int id)
        {
            //int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            //if (userId == 0)
            //{
            //    return BadRequest("Bạn cần đăng nhập để thanh toán.");
            //}
            //var order = _context.Orders.FirstOrDefault(o => o.UserId == id);
            var order = _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.UserId == id && o.Status != "Pending");
            if (order == null || order.UserId != id || order.Status != "Pending" || !order.IsActive)
            {
                return NotFound("Đơn hàng không tồn tại hoặc đã được thanh toán.");
            }

            // Trừ tồn kho cho từng sản phẩm trong đơn hàng
            foreach (var detail in order.OrderDetails)
            {
                var product = _context.Products.Find(detail.ProductId);
                if (product == null || !product.IsActive) continue;

                product.StockQuantity -= detail.Quantity;
                if (product.StockQuantity <= 0)
                {
                    product.StockQuantity = 0;
                    product.IsActive = false;

                    // Vô hiệu hóa các OrderDetail liên quan đến sản phẩm này
                    var relatedDetails = _context.OrderDetails.Where(od => od.ProductId == product.ProductId).ToList();
                    foreach (var rd in relatedDetails)
                    {
                        rd.IsActive = false; // giả sử bạn có cột IsActive trong OrderDetail
                    }
                }
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Finished";

            // Kiểm tra các đơn hàng khác
            var otherOrders = _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderId != id && o.Status == "Pending" && o.IsActive)
                .ToList();

            foreach (var otherOrder in otherOrders)
            {
                foreach (var detail in otherOrder.OrderDetails)
                {
                    var product = _context.Products.Find(detail.ProductId);
                    if (product == null || !product.IsActive) continue;

                    if (detail.Quantity > product.StockQuantity)
                    {
                        detail.Quantity = product.StockQuantity;
                        if (product.StockQuantity == 0)
                        {
                            detail.IsActive = false; // giả sử bạn có cột IsActive trong OrderDetail
                        }
                    }
                }
            }

            _context.SaveChanges();
            return Ok("Thanh toán thành công!");
        }

        // DELETE OrderDetail of ShoppingCart
        [HttpDelete("Home/DeleteOrderDetail/{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            //lay session nhan tu Login
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            if (userId == 0)
            {
                return BadRequest("Bạn cần đăng nhập để xóa sản phẩm khỏi giỏ hàng.");
            }
            var orderDetail = _context.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id && od.IsActive);
            if (orderDetail == null)
            {
                return NotFound("Chi tiết đơn hàng không tồn tại.");
            }

            // Xóa chi tiết đơn hàng
            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();

            // Reload lại order từ database
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == orderDetail.OrderId);

            // cap nhat lai tien trong Order
            order = _context.Orders.FirstOrDefault(o => o.UserId == userId && o.Status == "Pending" && o.IsActive);
            if (order != null)
            {
                // Tính lại tổng tiền của đơn hàng với discount
                decimal total = 0;
                foreach (var detail in order.OrderDetails)
                {
                    var product = _context.Products.Find(detail.ProductId);
                    if (product != null)
                    {
                        // Tính tổng tiền với discount
                        var discountPercent = _context.ProductDiscounts
                            .Where(pd => pd.ProductId == detail.ProductId && pd.IsActive)
                            .Join(_context.Discounts,
                                pd => pd.DiscountId,
                                d => d.DiscountId,
                                (pd, d) => new { pd, d })
                            .Select(x => (decimal?)x.d.DiscountPercent) // ép kiểu về decimal
                            .FirstOrDefault() ?? 0m;
                        total += detail.Quantity * product.Price * (1 - (discountPercent / 100m));
                    }
                }
                //// Xóa chi tiết đơn hàng
                //_context.OrderDetails.Remove(orderDetail);
                //_context.SaveChanges();

                //return Ok("Xóa sản phẩm khỏi giỏ hàng thành công!");
                //order.TotalAmount = order.OrderDetails
                //    .Where(od => od.IsActive)
                //    .Sum(od => od.Price * od.Quantity);
                _context.SaveChanges();
            }
            return Ok("Xóa sản phẩm khỏi giỏ hàng thành công!");
        }


        #endregion VIEW ShoppingCart





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
