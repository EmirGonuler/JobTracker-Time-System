using JobTracker.Core.Models;
using JobTracker.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data
{
    // AppDbContext is the "database connection manager".
    // It inherits from DbContext (provided by EF Core).
    // Think of it as the reception desk that routes requests to the right table.
    public class AppDbContext : DbContext
    {
        // This constructor receives "options" from outside (like the connection string).
        // We pass it up to the base DbContext class.
        // This is called "Dependency Injection" — the app tells the context how to connect.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Each DbSet is a table in your database.
        // DbSet<Job> means: "give me access to the Jobs table".
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimeCard> TimeCards { get; set; }

        // OnModelCreating lets us add extra rules and seed data.
        // "Seed data" = sample records added automatically when the DB is created.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call the base method first — EF Core needs it
            base.OnModelCreating(modelBuilder);

            // --- SEED DATA (sample records for testing) ---

            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, CompanyName = "Acme Corp", PhoneNumber = "0112345678", ContactName = "John Smith" },
                new Client { Id = 2, CompanyName = "TechSolve", PhoneNumber = "0219876543", ContactName = "Sarah Jones" },
                new Client { Id = 3, CompanyName = "Deloitte",  PhoneNumber = "0209846573", ContactName = "Melanie Scott" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Mike Johnson", EmployeeCode = "EMP001" },
                new Employee { Id = 2, FullName = "Lisa Brown",   EmployeeCode = "EMP002" },
                new Employee { Id = 3, FullName = "Robin McGee",  EmployeeCode = "EMP003" }
            );

            modelBuilder.Entity<Job>().HasData(
                new Job { Id = 1, JobNo = "R000001", JobType = JobType.Repair,   Date = new DateTime(2025, 1, 10), ClientId = 1 },
                new Job { Id = 2, JobNo = "S000001", JobType = JobType.Support,  Date = new DateTime(2025, 1, 12), ClientId = 2 },
                new Job { Id = 3, JobNo = "W000001", JobType = JobType.Warranty, Date = new DateTime(2025, 1, 13), ClientId = 3 }
            );

            modelBuilder.Entity<TimeCard>().HasData(
                new TimeCard { Id = 1, EmployeeId = 1, JobId = 1, DateWorked = new DateTime(2025, 1, 10), HoursWorked = 4.5m },
                new TimeCard { Id = 2, EmployeeId = 2, JobId = 1, DateWorked = new DateTime(2025, 1, 10), HoursWorked = 3.0m },
                new TimeCard { Id = 3, EmployeeId = 3, JobId = 3, DateWorked = new DateTime(2025, 1, 10), HoursWorked = 6.0m }
            );
        }
    }
}