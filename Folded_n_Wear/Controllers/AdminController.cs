using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Folded_n_Wear.Data; 
using Folded_n_Wear.Models; 
using Microsoft.AspNetCore.Mvc.Rendering;
using Mysqlx.Crud;

namespace Folded_n_Wear.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IActionResult MonthlyReport(int month, int year)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account"); 
            }

            var reports = _context.MonthlyReports
                .Where(r => r.DateOfReport.Month == month && r.DateOfReport.Year == year)
                .ToList();

            return View(reports);
        }
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Admin()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            return View("Admin");
        }
        public IActionResult ManageUsers()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            return View("ManageUsers"); 
        }
        public IActionResult ManageOrders()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            return View("ManageOrders");
        }
        public IActionResult ManagePayments()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            return View("ManagePayments"); 
        }

        [HttpGet]
        public IActionResult CreatePayment()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            var orders = _context.OrderSummaries.Select(o => new
            {
                o.OrderID,
                o.CustomerID,
                o.TotalAmount
            }).ToList();

            ViewBag.OrderList = new SelectList(orders, "OrderID", "OrderID");

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            var model = new AdminData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMonthly()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("LoginPage", "Account");
            }

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            // Calculate total earnings for paid orders in the current month
            var totalEarnings = _context.OrderSummaries
                .Where(o => o.DateReceived.Month == currentMonth && o.DateReceived.Year == currentYear
                            && _context.Payment.Any(p => p.OrderID == o.OrderID && p.PaymentStatus == "Paid"))
                .Sum(o => o.TotalAmount);

            // Calculate total kilos processed for paid orders in the current month
            var totalKilosProcessed = _context.OrderSummaries
                .Where(o => o.DateReceived.Month == currentMonth && o.DateReceived.Year == currentYear
                            && _context.Payment.Any(p => p.OrderID == o.OrderID && p.PaymentStatus == "Paid"))
                .Sum(o => o.Quantity);

            // Generate ReportID using year and month 
            var reportID = int.Parse($"{currentYear}{currentMonth:D2}");

            // Create a DateTime object for the first day of the current month
            var reportDate = new DateTime(currentYear, currentMonth, 1);

            var existingReport = _context.MonthlyReports
                .FirstOrDefault(r => r.ReportID == reportID);

            if (existingReport != null)
            {
                existingReport.TotalEarnings = totalEarnings;
                existingReport.TotalKilosProcessed = totalKilosProcessed;
                existingReport.DateOfReport = reportDate;
                _context.Update(existingReport);
                TempData["AlertMessage"] = "Monthly report for the current month has been updated.";
            }
            else
            {
                var newReport = new MonthlyReport
                {
                    ReportID = reportID,
                    TotalEarnings = totalEarnings,
                    TotalKilosProcessed = totalKilosProcessed,
                    DateOfReport = reportDate 
                };

                _context.Add(newReport);
                TempData["AlertMessage"] = "Monthly report for the current month has been created.";
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MonthlyReport));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AdminData adminData)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            if (!ModelState.IsValid)
            {
                return View(adminData);
            }

            return RedirectToAction("Admin", "Admin");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Role = HttpContext.Session.GetString("Role");
            var model = _context.Users
                .Where(c => c.customerID == id)
                .Select(c => new AdminData
                {
                    customerID = c.customerID,
                    email = c.email,
                    password = c.password,
                    custName = c.custName,
                    contactNo = c.contactNo,
                    address = c.address,
                    role = c.role
                }).FirstOrDefault();

            if (model == null)
            {
                return NotFound();
            }

            return View(model);

        }


        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.Users
                    .Where(u => u.role.ToLower() == "customer")
                    .Select(u => new
                    {
                        u.customerID,
                        u.custName,
                        u.email,
                        u.contactNo,
                        u.dateJoined
                    })
                    .ToListAsync();

                if (customers == null || customers.Count == 0)
                {
                    return Json(new { message = "No customers found." });
                }

                return Json(customers);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error retrieving customers", details = ex.Message });
            }
        }

        // DELETE CUSTOMER FUNCTIONALITY
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var customer = await _context.Users.FindAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Users.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Users.Remove(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageUsers"); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var order = await _context.OrderSummaries.FindAsync(id);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            _context.OrderSummaries.Remove(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost, ActionName("DeleteConfirms")]
        public async Task<IActionResult> DeleteConfirms(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
                return Json(new { success = false, message = "Payment not found." });

            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdatePaymentStatus(string paymentId, string newStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentId) || string.IsNullOrEmpty(newStatus))
                {
                    return Json(new { success = false, message = "Invalid payment ID or status." });
                }

                if (!int.TryParse(paymentId, out int paymentIdInt))
                {
                    return Json(new { success = false, message = "Invalid payment ID format." });
                }

                var payment = _context.Payment.Find(paymentIdInt);

                if (payment == null)
                {
                    return Json(new { success = false, message = "Payment not found." });
                }

                payment.PaymentStatus = newStatus;
                _context.SaveChanges();

                return Json(new { success = true, newStatus = payment.PaymentStatus });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        public class PaymentUpdateModel
        {
            public int PaymentID { get; set; }
            public string PaymentStatus { get; set; }
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(string orderId, string newStatus)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(newStatus))
            {
                return Json(new { success = false, message = "Invalid order ID or status." });
            }

            var order = _context.OrderSummaries.Find(orderId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.OrderStatus = newStatus;
            _context.SaveChanges();

            return Json(new { success = true, newStatus = order.OrderStatus });
        }


        [HttpPost]
        public IActionResult CreatePayment(PaymentData payment)
        {
            if (ModelState.IsValid)
            {
                _context.Payment.Add(payment);
                _context.SaveChanges();
                return RedirectToAction("ManagePayments");
            }

            return View(payment);
        }

        [HttpGet]
        public JsonResult GetOrderDetails(string orderID)
        {
            var order = _context.OrderSummaries
                                .Where(o => o.OrderID == orderID)
                                .FirstOrDefault();

            if (order != null)
            {
                return Json(new
                {
                    customerID = order.CustomerID,
                    balance = order.TotalAmount
                });
            }
            return Json(new { customerID = "", balance = 0 });
        }

        [HttpGet]
        public IActionResult GetOrders(DateTime? selectedDate)
        {
            var orders = _context.OrderSummaries
                .Select(o => new
                {
                    o.OrderID,
                    o.CustomerID,
                    o.DateReceived,
                    o.DatePickup,
                    o.IsRushed,
                    o.TotalAmount,
                    o.Quantity,
                    o.OrderStatus,
                    PaymentStatus = _context.Payment
                        .Where(p => p.OrderID == o.OrderID)
                        .Select(p => p.PaymentStatus)
                        .FirstOrDefault() ?? "Pending"
                });

            if (selectedDate.HasValue)
            {
                orders = orders.Where(o => o.DateReceived.Date == selectedDate.Value.Date);
            }

            return Json(orders.ToList());
        }


    }


}



