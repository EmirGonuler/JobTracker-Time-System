namespace JobTracker.Core.Models
{
    // This class is the "blueprint" for a Client.
    // Every client in the system will be created using this blueprint.
    public class Client
    {
        // The "Id" is the unique number that identifies each client in the database.
        // Every table needs a primary key — this is it.
        public int Id { get; set; }

        // "required" means this field cannot be left empty.
        // string? means it CAN be null/empty. string (no ?) means it CANNOT.
        public required string CompanyName { get; set; }

        public required string PhoneNumber { get; set; }

        public required string ContactName { get; set; }

        // This is a "navigation property" — it links this Client to all their Jobs.
        // Think of it like saying: "A client can have many jobs."
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
