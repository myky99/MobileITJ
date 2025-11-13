namespace MobileITJ.Models
{
    // This helper class combines a Job with the
    // name of the worker assigned to it.
    public class JobWithWorkerDetail
    {
        public Job Job { get; set; }
        public string WorkerName { get; set; } = "No worker assigned";
    }
}