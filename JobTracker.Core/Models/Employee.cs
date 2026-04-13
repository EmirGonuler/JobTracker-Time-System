namespace JobTracker.Core.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public required string FullName { get; set; }

        public required string EmployeeCode { get; set; }

        // One employee can have many time cards (one per day per job)
        public ICollection<TimeCard> TimeCards { get; set; } = new List<TimeCard>();
    }
}
