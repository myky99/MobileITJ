using System;

namespace MobileITJ.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int WorkerUserId { get; set; }
        public int JobId { get; set; }
        public string JobDescription { get; set; } = "";
        public decimal AmountPaid { get; set; }
        public DateTime DatePaid { get; set; }
    }
}