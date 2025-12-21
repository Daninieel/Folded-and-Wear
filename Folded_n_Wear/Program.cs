using Microsoft.EntityFrameworkCore;
using Folded_n_Wear.Data;
using Folded_n_Wear.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Required for session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Session timeout
});

// Configure the DbContext using your connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

//cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/LoginPage";  // Redirect unauthenticated users
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Set cookie expiration
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Create a scope to perform database migration and seeding.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Apply any pending migrations.
        context.Database.Migrate();
        Console.WriteLine("Database migration completed.");

        // Optionally verify the connection.
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful!");
        }
        else
        {
            Console.WriteLine("Database connection failed.");
        }

        // Seed the admin user if it doesn't already exist.
        if (!context.Users.Any(u => u.role == "Admin"))
        {
            var passwordHasher = new PasswordHasher<MyData>();
            var adminUser = new MyData
            {
                email = "admin@foldednwear.com",
                password = passwordHasher.HashPassword(null, "admin123"), // Hash the admin password
                custName = "Admin User",
                contactNo = "09123456789",
                address = "Admin Address",
                dateJoined = DateTime.UtcNow,
                role = "Admin"
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
            Console.WriteLine("Admin account created: Email = admin@foldednwear.com | Password = admin123");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating the database: {ex.Message}");
    }
}

app.UseSession();
app.UseAuthentication(); // Must be before Authorization


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LoginPage}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Admin}/{id?}");

app.Run();
