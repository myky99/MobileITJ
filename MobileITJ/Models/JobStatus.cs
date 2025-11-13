namespace MobileITJ.Models
{
    public enum JobStatus
    {
        Open,       // Newly created, looking for workers
        Ongoing,    // Workers assigned, job in progress
        Completed,  // Customer marked as complete
        Incomplete  // Customer marked as incomplete
    }
}