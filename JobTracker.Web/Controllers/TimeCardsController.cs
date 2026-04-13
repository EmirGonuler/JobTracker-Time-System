using JobTracker.Data;
using JobTracker.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace JobTracker.Web.Controllers
{
    // This controller handles clocking in time cards for jobs.
    //
    // URL examples:
    //   GET  /TimeCards/Create?jobId=5  → shows the clock-in form for job ID 5
    //   POST /TimeCards/Create          → saves the time card and redirects back
    public class TimeCardsController : Controller
    {
        private readonly TimeCardService _timeCardService;
        private readonly AppDbContext _context;

        public TimeCardsController(TimeCardService timeCardService, AppDbContext context)
        {
            _timeCardService = timeCardService;
            _context = context;
        }

        // ---------------------------------------------------------------
        // GET: /TimeCards/Create?jobId=5
        // Shows the clock-in form for a specific job
        // ---------------------------------------------------------------
        public IActionResult Create(int jobId)
        {
            try
            {
                // Pass the jobId to the view so the form knows which job
                // it is submitting for
                ViewBag.JobId = jobId;

                // Load the employee dropdown
                PopulateEmployeeDropdown();

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not load clock-in form: {ex.Message}";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
        }

        // ---------------------------------------------------------------
        // POST: /TimeCards/Create
        // Saves the time card when the user clicks "Clock In"
        //
        // IMPORTANT: hoursWorked is received as a STRING on purpose.
        // This is because different computers use different decimal separators.
        // South Africa / Windows may send "7,5" instead of "7.5".
        // By accepting a string and parsing it ourselves with InvariantCulture,
        // we guarantee "7.5" always works regardless of regional settings.
        // ---------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int jobId, int employeeId, DateTime dateWorked, string hoursWorked)
        {
            try
            {
                // Parse the hours string using InvariantCulture so that
                // "7.5" always works — even if Windows is set to use commas.
                if (!decimal.TryParse(
                        hoursWorked,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out decimal parsedHours))
                {
                    TempData["ErrorMessage"] = "Please enter a valid number for hours worked (e.g. 7.5).";
                    ViewBag.JobId = jobId;
                    PopulateEmployeeDropdown(employeeId);
                    return View();
                }

                _timeCardService.ClockIn(employeeId, jobId, dateWorked, parsedHours);

                TempData["SuccessMessage"] = "Time card saved successfully!";

                // After saving, send the user back to that job's details page
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
            catch (Exception ex)
            {
                // Show full error detail so we can debug if needed
                string errorDetail = ex.InnerException != null
                    ? $"{ex.Message} | Detail: {ex.InnerException.Message}"
                    : ex.Message;

                TempData["ErrorMessage"] = $"Could not save time card: {errorDetail}";
                ViewBag.JobId = jobId;
                PopulateEmployeeDropdown(employeeId);
                return View();
            }
        }

        // ---------------------------------------------------------------
        // PRIVATE HELPER — loads employees into a dropdown list
        // ---------------------------------------------------------------
        private void PopulateEmployeeDropdown(int? selectedEmployeeId = null)
        {
            var employees = _context.Employees
                .OrderBy(e => e.FullName)
                .ToList();

            // "FullName" is what the user sees in the dropdown
            ViewBag.EmployeeId = new SelectList(employees, "Id", "FullName", selectedEmployeeId);
        }

        // ---------------------------------------------------------------
        // POST: /TimeCards/Delete/5
        // Deletes a time card and redirects back to the job details page.
        // We use POST (not GET) for deletions — a GET delete means someone
        // could delete records just by sharing a link, which is dangerous.
        // ---------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                // DeleteTimeCard returns the jobId so we know where to redirect
                int jobId = _timeCardService.DeleteTimeCard(id);

                TempData["SuccessMessage"] = "Time card removed successfully.";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Could not remove time card: {ex.Message}";
                return RedirectToAction("Index", "Jobs");
            }
        }
    }
}