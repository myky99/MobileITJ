using System;

namespace MobileITJ.Models
{
    public class JobReport
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CustomerId { get; set; }
        public string JobDescription { get; set; } = "";
        public string ReportMessage { get; set; } = "";
        public DateTime DateFiled { get; set; }
    }
}