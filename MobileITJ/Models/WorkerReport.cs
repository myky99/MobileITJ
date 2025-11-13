using System;

namespace MobileITJ.Models
{
    public class WorkerReport
    {
        public int Id { get; set; }
        public int WorkerUserId { get; set; }
        public string WorkerName { get; set; } = "";
        public int CustomerId { get; set; } // The ID of the user who filed the report
        public string ReportMessage { get; set; } = "";
        public DateTime DateFiled { get; set; }
    }
}