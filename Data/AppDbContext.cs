using CourierManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;




namespace CourierManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }




        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Invoice> Invoices { get; set; }


        public DbSet<DeletedCustomerLog> DeletedCustomerLogs { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("Admins");
            modelBuilder.Entity<Customer>()
                .Property(c => c.PhoneNumber)
                .IsRequired();  // makes it NOT NULL in DB            modelBuilder.Entity<Parcel>().ToTable("Parcels");
            modelBuilder.Entity<Parcel>().Property(p => p.TotalCost).HasPrecision(18, 2);


            modelBuilder.Entity<Invoice>()
             .HasOne(i => i.Parcel)
             .WithMany()
             .HasForeignKey(i => i.ParcelId)
             .OnDelete(DeleteBehavior.NoAction);  // ← fixes the error


            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany()
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);  // ← same for Customer
        }
    }
}
