using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Folded_n_Wear.Data; 
using Folded_n_Wear.Models;


namespace Folded_n_Wear.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<MyData> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<MyData>();
        }

        public IActionResult GetSession()
        {
            var value = HttpContext.Session.GetString("TestKey");
            return Content($"Session Value: {value}");
        }

        public IActionResult GetCookie()
        {
            var cookieValue = Request.Cookies["MyCookie"];
            return Content($"Cookie Value: {cookieValue}");
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string custName, string contactNo, string address)
        {
            
            if (await _context.Users.AnyAsync(u => u.email == email))
            {
                ModelState.AddModelError("", "Email already exists.");

                
                ViewBag.Role = HttpContext.Session.GetString("Role");

                
                if (HttpContext.Session.GetString("Role") == "Admin")
                {
                    return RedirectToAction("Admin", "Admin"); 
                }
                else
                {
                    return RedirectToAction("Register", "Account"); 
                }
            }

            var user = new MyData
            {
                email = email,
                password = _passwordHasher.HashPassword(new MyData(), password),
                custName = custName,
                contactNo = contactNo,
                address = address,
                dateJoined = DateTime.UtcNow,
                role = "Customer" 
            };

            // Add the new user to the context and save changes
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Get the role from the session again after registration
            ViewBag.Role = HttpContext.Session.GetString("Role");

            // Redirect based on the role after successful registration
            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                return RedirectToAction("Admin", "Admin");
            }
            else
            {
                return RedirectToAction("LoginPage", "Account"); 
            }
        }

        // Account/Edit/{id}
        public async Task<IActionResult> Edit(int id) 
        {
            var user = await _context.Users.FindAsync(id); 
            if (user == null)
            {
                return NotFound(); 
            }

            // Map user properties to AdminData
            var model = new AdminData
            {
                email = user.email,
                custName = user.custName,
                contactNo = user.contactNo,
                address = user.address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string email, string password, string custName, string contactNo, string address, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);

            if (user == null)
            {
                return RedirectToAction("Edit", new { email, error = "User not found." });
            }

            user.custName = custName;
            user.contactNo = contactNo;
            user.address = address;
            user.role = role;

            if (!string.IsNullOrEmpty(password))
            {
                user.password = _passwordHasher.HashPassword(user, password);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Admin", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View("LoginPage");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Store session data
                    HttpContext.Session.SetString("Email", user.email);
                    HttpContext.Session.SetString("custName", user.custName);
                    HttpContext.Session.SetString("Role", user.role);
                    HttpContext.Session.SetInt32("CustomerID", user.customerID);

                    if (rememberMe)
                    {
                        CookieOptions options = new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(7),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = true
                        };

                        Response.Cookies.Append("Email", user.email, options);
                        Response.Cookies.Append("custName", user.custName, options);
                        Response.Cookies.Append("Role", user.role, options);
                        Response.Cookies.Append("CustomerID", user.customerID.ToString(), options);
                    }

                    if (user.role == "Admin")
                    {
                        return RedirectToAction("Admin", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("HomePage", "Home");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View("LoginPage");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Email");
            Response.Cookies.Delete("custName");
            Response.Cookies.Delete("Role");
            Response.Cookies.Delete("CustomerID");

            return RedirectToAction("LoginPage", "Account");
        }

    }
}