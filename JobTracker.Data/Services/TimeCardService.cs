using JobTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data.Services
{
    public class TimeCardService
    {
        private readonly AppDbContext _context;

        public TimeCardService(AppDbContext context)
        {
            _context = context;
        }

        // Returns all time cards for a specific job
        public List<TimeCard> GetTimeCardsForJob(int jobId)
        {
            try
            {
                return _context.TimeCards
                    .Include(tc => tc.Employee)
                    .Where(tc => tc.JobId == jobId)
                    .OrderBy(tc => tc.DateWorked)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve time cards for job {jobId}.", ex);
            }
        }

        // Clock in: record hours worked by an employee on a specific job
        public TimeCard ClockIn(int employeeId, int jobId, DateTime dateWorked, decimal hoursWorked)
        {
            try
            {
                // Validate that the job exists
                var jobExists = _context.Jobs.Any(j => j.Id == jobId);
                if (!jobExists)
                    throw new ArgumentException($"Job with ID {jobId} does not exist.");

                // Validate that the employee exists
                var employeeExists = _context.Employees.Any(e => e.Id == employeeId);
                if (!employeeExists)
                    throw new ArgumentException($"Employee with ID {employeeId} does not exist.");

                // Validate hours — must be between 0.5 and 24
                if (hoursWorked < 0.5m || hoursWorked > 24m)
                    throw new ArgumentException($"Hours worked must be between 0.5 and 24. You entered: {hoursWorked}");

                var timeCard = new TimeCard
                {
                    EmployeeId = employeeId,
                    JobId = jobId,
                    DateWorked = dateWorked,
                    HoursWorked = hoursWorked
                };

                _context.TimeCards.Add(timeCard);
                _context.SaveChanges();

                return timeCard;
            }
            catch (ArgumentException)
            {
                // Re-throw validation errors as-is so the message reaches the UI
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to clock in time card.", ex);
            }
        }

        // Deletes a single time card entry by its ID.
        // Returns the JobId so the caller can redirect back to the correct job details page.
        public int DeleteTimeCard(int id)
        {
            try
            {
                var timeCard = _context.TimeCards.Find(id)
                    ?? throw new KeyNotFoundException($"Time card with ID {id} not found.");

                // Save the jobId before deleting so we can redirect back to it
                int jobId = timeCard.JobId;

                _context.TimeCards.Remove(timeCard);
                _context.SaveChanges();

                return jobId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete time card.", ex);
            }
        }
    }
}
