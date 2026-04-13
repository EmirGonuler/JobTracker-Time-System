using JobTracker.Core.Models;
using JobTracker.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data.Services
{
    // JobService handles all Create/Read/Update operations for Jobs.
    // Notice: NO delete — the requirements didn't ask for it.
    public class JobService
    {
        private readonly AppDbContext _context;
        private readonly JobNumberService _jobNumberService;

        // We receive BOTH dependencies through the constructor.
        // This makes the class easy to test and easy to read.
        public JobService(AppDbContext context, JobNumberService jobNumberService)
        {
            _context = context;
            _jobNumberService = jobNumberService;
        }

        // Returns ALL jobs, including their related Client data
        public List<Job> GetAllJobs()
        {
            try
            {
                // "Include" tells EF Core to also fetch the related Client record.
                // Without Include, Job.Client would be null.
                return _context.Jobs
                    .Include(j => j.Client)
                    .OrderByDescending(j => j.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve jobs.", ex);
            }
        }

        // Returns a single job by its Id, or null if not found
        public Job? GetJobById(int id)
        {
            try
            {
                return _context.Jobs
                    .Include(j => j.Client)
                    .Include(j => j.TimeCards)
                        .ThenInclude(tc => tc.Employee)
                    .FirstOrDefault(j => j.Id == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve job with ID {id}.", ex);
            }
        }

        // Creates a new job in the database
        public Job CreateJob(JobType jobType, DateTime date, int clientId)
        {
            try
            {
                // Step 1: Generate the job number (e.g. "R000003")
                string jobNo = _jobNumberService.GenerateJobNumber(jobType);

                // Step 2: Build the new Job object
                var job = new Job
                {
                    JobNo = jobNo,
                    JobType = jobType,
                    Date = date,
                    ClientId = clientId
                };

                // Step 3: Add to the context (marks it as "pending insert")
                _context.Jobs.Add(job);

                // Step 4: SaveChanges() sends the INSERT statement to SQL Server
                _context.SaveChanges();

                return job;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create job.", ex);
            }
        }

        // Updates an existing job's details
        public void UpdateJob(int id, JobType jobType, DateTime date, int clientId)
        {
            try
            {
                var job = _context.Jobs.Find(id)
                    ?? throw new KeyNotFoundException($"Job with ID {id} not found.");

                // Only update the fields that are allowed to change.
                // JobNo does NOT change — it was set at creation.
                job.JobType = jobType;
                job.Date = date;
                job.ClientId = clientId;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update job.", ex);
            }
        }

        // Returns all clients — used to populate dropdown lists in forms
        public List<Client> GetAllClients()
        {
            try
            {
                return _context.Clients
                    .OrderBy(c => c.CompanyName)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve clients.", ex);
            }
        }

        // Deletes a job and all its associated time cards from the database.
        // We delete time cards first because they depend on the job (foreign key).
        // Deleting the job first would cause a database constraint error.
        public void DeleteJob(int id)
        {
            try
            {
                var job = _context.Jobs
                    .Include(j => j.TimeCards)
                    .FirstOrDefault(j => j.Id == id)
                    ?? throw new KeyNotFoundException($"Job with ID {id} not found.");

                // Remove all time cards linked to this job first
                _context.TimeCards.RemoveRange(job.TimeCards);

                // Now safe to remove the job itself
                _context.Jobs.Remove(job);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to delete job.", ex);
            }
        }
    }
}