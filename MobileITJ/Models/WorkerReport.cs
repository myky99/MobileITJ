using System;

namespace MobileITJ.Models
{
    public class WorkerReport
    {
        public int Id { get; set; }

        // Worker Info
        public int WorkerUserId { get; set; }
        public string WorkerName { get; set; } = "";

        // Customer Info (The Reporter)
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = "Unknown";
        public string CustomerEmail { get; set; } = "";

        // Job Info (The Context)
        public int JobId { get; set; }
        public string JobDescription { get; set; } = "General Report";

        public string ReportMessage { get; set; } = "";
        public DateTime DateFiled { get; set; }
    }
}