namespace JobTracker.Core.Enums
{
    // An enum is like a list of allowed choices.
    // Instead of typing the string "Repair" everywhere (and risking typos),
    // we use JobType.Repair — clean, safe, and easy to read.
    public enum JobType
    {
        Repair,
        Support,
        Warranty
    }
}
