using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Data
{
    // This class is ONLY used by EF Core at design time (migrations).
    // It is never called by your actual running application.
    // Think of it as a spare key hidden under the mat — only used when needed.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // This is the connection string used ONLY for running migrations.
            // It points to a local SQL Server database on your machine.
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=JobTrackerDb;Trusted_Connection=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
