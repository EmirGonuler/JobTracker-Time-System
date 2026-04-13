using JobTracker.Core.Enums;
using JobTracker.Core.Models;
using JobTracker.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobTracker.Web.Controllers
{
    // This controller handles all web pages related to Jobs.
    // Each method below is called an "Action" — it handles one specific URL.
    //
    // URL examples this controller handles:
    //   GET  /Jobs          → Index()   — shows the list of all jobs
    //   GET  /Jobs/Create   → Create()  — shows the blank create form
    //   POST /Jobs/Create   → Create()  — handles the form being submitted
    //   GET  /Jobs/Edit/5   → Edit(5)   — shows the edit form for job ID 5
    //   POST /Jobs/Edit/5   → Edit(5)   — handles the edit form being submitted
    //   GET  /Jobs/Details/5→ Details(5)— shows details for job ID 5
    public class JobsController : Controller
    {
        private readonly JobService _jobService;

        // ASP.NET automatically provides (injects) these services.
        // We never call "new JobService()" ourselves — the framework does it for us.
        public JobsController(JobService jobService)
        {
            _jobService = jobService;
        }

        // ---------------------------------------------------------------
        // GET: /Jobs
        // Shows a table of all jobs
        // ---------------------------------------------------------------
        public IActionResult Index()
        {
            try
            {
                var jobs = _jobService.GetAllJobs();
                return View(jobs);
            }
            catch (Exception ex)
            {
                // TempData survives one redirect — perfect for showing
                // error messages after something goes wrong
                TempData["ErrorMessage"] = $"Could not load jobs: {ex.Message}";
                return View(new List<Job>());
            }
        }

        // ---------------------------------------------------------------
        // GET: /Jobs/Details/5
        // Shows all information about one specific job, including its time cards
        // ---------------------------------------------------------------
        public IActionResult Details(int id)
        {
            try
            {
                var job = _jobService.GetJobById(id);

                if (job == null)
                {
                    TempData["ErrorMessage"] = $"Job with ID {id} was not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(job);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not load job details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------------------------------------------------------
        // GET: /Jobs/Create
        // Shows the blank "Create New Job" form
        // ---------------------------------------------------------------
        public IActionResult Create()
        {
            try
            {
                // Load the client dropdown list and pass it to the view
                PopulateClientDropdown();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not load create form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------------------------------------------------------
        // POST: /Jobs/Create
        // Handles the form submission when user clicks "Save"
        // The [HttpPost] attribute means this only runs when a form is submitted
        // ---------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(JobType jobType, DateTime date, int clientId)
        {
            try
            {
                _jobService.CreateJob(jobType, date, clientId);

                TempData["SuccessMessage"] = "Job created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not create job: {ex.Message}";
                PopulateClientDropdown();
                return View();
            }
        }

        // ---------------------------------------------------------------
        // GET: /Jobs/Edit/5
        // Shows the edit form pre-filled with the job's current data
        // ---------------------------------------------------------------
        public IActionResult Edit(int id)
        {
            try
            {
                var job = _jobService.GetJobById(id);

                if (job == null)
                {
                    TempData["ErrorMessage"] = $"Job with ID {id} was not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Pass the client dropdown AND the selected client ID to the view
                PopulateClientDropdown(job.ClientId);
                return View(job);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not load edit form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------------------------------------------------------
        // POST: /Jobs/Edit/5
        // Handles the edit form submission
        // ---------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, JobType jobType, DateTime date, int clientId)
        {
            try
            {
                _jobService.UpdateJob(id, jobType, date, clientId);

                TempData["SuccessMessage"] = "Job updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not update job: {ex.Message}";
                PopulateClientDropdown(clientId);
                return View();
            }
        }

        // ---------------------------------------------------------------
        // PRIVATE HELPER METHOD
        // Loads the list of clients from the database and puts them into
        // ViewBag so the dropdown in the view can display them.
        // Having this as a separate method means we don't repeat the same
        // code in Create, Edit, and any other action that needs the dropdown.
        // ---------------------------------------------------------------
        private void PopulateClientDropdown(int? selectedClientId = null)
        {
            var clients = _jobService.GetAllClients();

            // SelectList turns a list of objects into dropdown-friendly items
            // "Id" = the value stored when selected
            // "CompanyName" = the text the user sees in the dropdown
            ViewBag.ClientId = new SelectList(clients, "Id", "CompanyName", selectedClientId);
        }

        // ---------------------------------------------------------------
        // GET: /Jobs/Delete/5
        // Shows a confirmation page before deleting — always confirm
        // before any destructive action so users don't delete by accident
        // ---------------------------------------------------------------
        public IActionResult Delete(int id)
        {
            try
            {
                var job = _jobService.GetJobById(id);

                if (job == null)
                {
                    TempData["ErrorMessage"] = $"Job with ID {id} was not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(job);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not load job: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ---------------------------------------------------------------
        // POST: /Jobs/Delete/5
        // Performs the actual deletion after user confirms
        // ---------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _jobService.DeleteJob(id);

                TempData["SuccessMessage"] = "Job deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not delete job: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}