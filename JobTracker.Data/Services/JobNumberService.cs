using JobTracker.Core.Enums;

namespace JobTracker.Data.Services
{
    // This service has ONE job: generate job numbers like "R000001", "S000002", "W000003"
    // Having this in its own class means if the format ever changes,
    // you only change it HERE — not in 10 different places.
    public class JobNumberService
    {
        private readonly AppDbContext _context;

        // The AppDbContext is "injected" here — we don't create it ourselves.
        // The system gives it to us. This is Dependency Injection.
        public JobNumberService(AppDbContext context)
        {
            _context = context;
        }

        // Generates the next job number for a given job type
        // Example: if there are 3 Repair jobs, returns "R000004"
        public string GenerateJobNumber(JobType jobType)
        {
            try
            {
                // Get the first letter prefix based on job type
                char prefix = jobType switch
                {
                    JobType.Repair => 'R',
                    JobType.Support => 'S',
                    JobType.Warranty => 'W',
                    // This handles any unexpected value safely
                    _ => throw new ArgumentOutOfRangeException(nameof(jobType), "Unknown job type")
                };

                // Count how many jobs of this type already exist
                int count = _context.Jobs
                    .Count(j => j.JobType == jobType);

                // PadLeft(6, '0') adds leading zeros: 1 becomes "000001"
                string number = (count + 1).ToString().PadLeft(6, '0');

                return $"{prefix}{number}";
            }
            catch (Exception ex)
            {
                // In a real app you'd log this. For now, we wrap and rethrow
                // so the caller knows something went wrong.
                throw new InvalidOperationException("Failed to generate job number.", ex);
            }
        }
    }
}
