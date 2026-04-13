namespace JobTracker.Core.Models
{
    public class TimeCard
    {
        public int Id { get; set; }

        // Foreign key to Employee
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        // Foreign key to Job
        public int JobId { get; set; }
        public Job? Job { get; set; }

        public DateTime DateWorked { get; set; }

        // decimal is used for money or precise numbers.
        // We use it here because someone might work 7.5 hours.
        public decimal HoursWorked { get; set; }
    }
}
