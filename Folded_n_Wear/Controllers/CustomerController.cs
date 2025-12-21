using Folded_n_Wear.Data;
using Folded_n_Wear.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor to initialize ApplicationDbContext
    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult GetSession()
    {
        var value = HttpContext.Session.GetString("Email");
        return Content($"Session Value: {value}");
    }

    public IActionResult GetCookie()
    {
        var cookieValue = Request.Cookies["MyCookie"];
        return Content($"Cookie Value: {cookieValue}");
    }

    public IActionResult HomePage()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email")))
        {
            return RedirectToAction("LoginPage", "Account");
        }

        ViewBag.Email = HttpContext.Session.GetString("Email");
        ViewBag.customerID = HttpContext.Session.GetString("customerID");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.custName = HttpContext.Session.GetString("custName");
        ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
        ViewBag.address = HttpContext.Session.GetString("address");
        return View();
    }

    public IActionResult Booking()
    {
        if (HttpContext.Session.GetString("Email") == null)
        {
            return RedirectToAction("LoginPage", "Account");
        }
        ViewBag.Email = HttpContext.Session.GetString("Email");
        ViewBag.customerID = HttpContext.Session.GetString("customerID");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.custName = HttpContext.Session.GetString("custName");
        ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
        ViewBag.address = HttpContext.Session.GetString("address");
        return View();
    }
    public IActionResult MyBookings()
    {
        if (HttpContext.Session.GetString("Email") == null)
        {
            return RedirectToAction("LoginPage", "Account");
        }
        ViewBag.Email = HttpContext.Session.GetString("Email");
        ViewBag.customerID = HttpContext.Session.GetString("customerID");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.custName = HttpContext.Session.GetString("custName");
        ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
        ViewBag.address = HttpContext.Session.GetString("address");

        // Retrieve the customer ID from the session
        var customerId = HttpContext.Session.GetInt32("CustomerID");

        // Log the retrieved customerID for debugging
        Console.WriteLine($"Retrieved customerID from session: {customerId}");

        if (!customerId.HasValue)
        {
            return Json(new { error = true, message = "User not logged in or invalid customer ID" });
        }
        
        Console.WriteLine($"Retrieved CustomerID from session: {customerId.Value}");

        // Fetch the bookings for the current user
        var bookings = _context.OrderSummaries
            .Where(b => b.CustomerID == customerId.Value)
            .Select(b => new
            {
                b.OrderID,
                b.CustomerID,
                b.DateReceived,
                b.DatePickup,
                b.IsRushed, 
                b.OrderStatus,
                b.TotalAmount
            })
            .ToList();

        Console.WriteLine($"Fetched {bookings.Count} payments for CustomerID: {customerId.Value}");

        return View(bookings);
    }

    public IActionResult Items()
    {
        if (HttpContext.Session.GetString("Email") == null)
        {
            return RedirectToAction("LoginPage", "Account");
        }
        ViewBag.Email = HttpContext.Session.GetString("Email");
        ViewBag.customerID = HttpContext.Session.GetString("customerID");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.custName = HttpContext.Session.GetString("custName");
        ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
        ViewBag.address = HttpContext.Session.GetString("address");
        return View();
    }
    public IActionResult Payment()
    {
        if (HttpContext.Session.GetString("Email") == null)
        {
            return RedirectToAction("LoginPage", "Account");
        }
        ViewBag.Email = HttpContext.Session.GetString("Email");
        ViewBag.customerID = HttpContext.Session.GetString("customerID");
        ViewBag.Role = HttpContext.Session.GetString("Role");
        ViewBag.custName = HttpContext.Session.GetString("custName");
        ViewBag.contactNo = HttpContext.Session.GetString("contactNo");
        ViewBag.address = HttpContext.Session.GetString("address");

        var customerId = HttpContext.Session.GetInt32("CustomerID");

        if (!customerId.HasValue)
        {
            // If customerId is not found in the session, redirect to login page
            return RedirectToAction("LoginPage", "Account");
        }

        // Fetch payments for the customer
        var payments = _context.Payment
            .Where(p => p.CustomerID == customerId.Value)
            .Select(p => new
            {
                p.PaymentID,
                p.OrderID,
                p.PaymentStatus,
                p.PaymentDate,
                p.balance
            })
            .ToList();

        return View(payments);
    }

    public async Task<IActionResult> OrderSummary(
    string OrderID,
    int CustomerID,
    string DateReceived,
    string DatePickup,
    string TotalAmount,
    string IsRushed,
    int Quantity
)
    {
        Console.WriteLine($"Received Quantity: {Quantity}");

        if (Quantity == 0)
        {
            Console.WriteLine("⚠️ Quantity is missing or zero!");
            return BadRequest("Quantity must be greater than zero.");
        }

        decimal totalAmountDecimal = Convert.ToDecimal(TotalAmount);
        DateTime dateReceived = Convert.ToDateTime(DateReceived);
        DateTime datePickup = Convert.ToDateTime(DatePickup);

        // Calculate total kilos booked for the selected pickup date
        decimal totalKilosOnPickupDate = _context.OrderSummaries
            .Where(o => o.DatePickup.Date == datePickup.Date)
            .Sum(o => o.Quantity);

        // If adding this order exceeds 10 kilos, check the rushed logic
        if (totalKilosOnPickupDate + Quantity > 10)
        {
            if (IsRushed == "Yes")
            {
                DateTime nextDay = datePickup.AddDays(1);
                decimal nextDayTotalKilos = _context.OrderSummaries
                    .Where(o => o.DatePickup.Date == nextDay.Date)
                    .Sum(o => o.Quantity);

                if (nextDayTotalKilos + Quantity > 10)
                {
                    TempData["AlertMessage"] = "Cannot rush order. This pick up date capacity is full.";
                    return RedirectToAction("Booking");
                }

                // Move order to the next day
                datePickup = nextDay;
            }
            else
            {
                // Display the specific pickup date
                TempData["AlertMessage"] = $"Date pickup {datePickup.ToString("MMMM dd, yyyy")} is full. Please choose another date.";
                return RedirectToAction("Booking");
            }
        }

        var orderSummary = new OrderSummaryViewModel
        {
            OrderID = OrderID,
            CustomerID = CustomerID,
            DateReceived = dateReceived,
            DatePickup = datePickup,
            TotalAmount = totalAmountDecimal,
            IsRushed = IsRushed,
            Quantity = Quantity
        };

        Console.WriteLine($"Total kilos already booked for {datePickup}: {totalKilosOnPickupDate}");

        _context.OrderSummaries.Add(orderSummary);
        await _context.SaveChangesAsync();

        return View(orderSummary);
    }

    private string GetItemName(int itemTypeId)
    {
        return itemTypeId switch
        {
            1 => "Regular Clothes (₱28.00/kilo)",
            2 => "Blanket, Towels, Linen (₱40.00/kilo)",
            3 => "Comforter, Heavy Towels, Stuffed Toys (₱65.00/kilo)",
            _ => "Unknown Item"
        };
    }
    public IActionResult ViewBookings()
    {
        var userId = HttpContext.Session.GetString("customerID");
        if (int.TryParse(userId, out int customerId))
        {
            var orders = _context.OrderSummaries
                .Where(o => o.CustomerID == customerId)
                .Join(_context.Payment,
                    order => order.OrderID,
                    payment => payment.OrderID,
                    (order, payment) => new
                    {
                        order.OrderID,
                        order.CustomerID,
                        order.DatePickup,
                        order.TotalAmount,
                        order.OrderStatus,
                        payment.PaymentStatus,
                        payment.PaymentDate
                    })
                .ToList();

            return View(orders);
        }
        else
        {
            return RedirectToAction("Login", "Account"); 
        }

    }

    public IActionResult PaymentDetails()
    {
        var userIdString = HttpContext.Session.GetString("customerID");
        int userId = Convert.ToInt32(userIdString);

        // Fetch orders and payments for the logged-in user
        var orders = _context.OrderSummaries
            .Where(o => o.CustomerID == userId)
            .Join(_context.Payment,
                order => order.OrderID,
                payment => payment.OrderID,
                (order, payment) => new
                {
                    order.OrderID,
                    order.DatePickup,
                    order.TotalAmount,
                    order.OrderStatus,
                    payment.PaymentStatus,
                    payment.PaymentDate
                })
            .ToList();

        if (orders == null || !orders.Any())
        {

            ViewBag.ErrorMessage = "No orders found or payments not made yet.";
        }

        return View(orders);
    }

    public IActionResult GetPayments()
    {
        var payments = _context.Payment.ToList(); 
        return Json(payments);
    }
    public IActionResult GetPayment()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerID");

        if (!customerId.HasValue)
        {
            return Json(new { error = true, message = "User not logged in" });
        }

        var payments = _context.Payment
            .Where(p => p.CustomerID == customerId.Value)
            .Select(p => new
            {
                p.PaymentID,
                p.CustomerID,
                p.OrderID,
                p.PaymentDate,
                p.PaymentStatus,
                p.balance
            })
            .ToList();

        return Json(payments);
    }


    public IActionResult GetOrders()
    {
        var orders = _context.OrderSummaries.ToList();  // Fetching all orders from the database
        return Json(orders);
    }

    public IActionResult GetOrder()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerID");

        if (!customerId.HasValue)
        {
            return Json(new { error = true, message = "No customer ID found" });
        }

        var orders = _context.OrderSummaries
            .Where(o => o.CustomerID == customerId.Value)
            .Select(o => new
            {
                o.OrderID,
                o.CustomerID,
                o.DateReceived,
                o.DatePickup,
                o.IsRushed, 
                o.TotalAmount,
                o.OrderStatus
            })
            .ToList();

        return Json(orders);
    }

    [HttpGet]
    public JsonResult GetTotalKilosForDate(DateTime pickupDate)
    {
        var totalKilos = _context.OrderSummaries
            .Where(o => o.DatePickup.Date == pickupDate.Date)
            .Sum(o => o.Quantity);

        return Json(new { totalKilos });
    }
    [HttpPost]
    public JsonResult CancelOrder(string orderID)
    {
        var order = _context.OrderSummaries.FirstOrDefault(o => o.OrderID == orderID);
        if (order == null)
        {
            return Json(new { success = false, error = "Order not found." });
        }

        if (order.OrderStatus == "Completed" || order.OrderStatus == "Canceled")
        {
            return Json(new { success = false, error = "Order cannot be canceled as it is already completed or canceled." });
        }

        order.OrderStatus = "Canceled";

        _context.SaveChanges();

        return Json(new { success = true });
    }

}




