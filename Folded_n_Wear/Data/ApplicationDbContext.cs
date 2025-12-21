using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Folded_n_Wear.Models;
using System.Collections.Generic;

namespace Folded_n_Wear.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<MyData> Users { get; set; }
        public DbSet<OrderSummaryViewModel> OrderSummaries { get; set; }
        public DbSet<ItemViewModel> Items { get; set; }
        public DbSet<PaymentData> Payment { get; set; }

        public DbSet<MonthlyReport> MonthlyReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyData>().ToTable("user");

            modelBuilder.Entity<OrderSummaryViewModel>().ToTable("order");

            modelBuilder.Entity<ItemViewModel>().ToTable("item");

            modelBuilder.Entity<PaymentData>().ToTable("payment");

            modelBuilder.Entity<MonthlyReport>().ToTable("monthly_report");

        }
    }
}
