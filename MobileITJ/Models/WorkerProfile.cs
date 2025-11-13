using System.Collections.Generic;

namespace MobileITJ.Models;
public class WorkerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string WorkerId { get; set; } = "";
    public List<string> Skills { get; set; } = new();
    public decimal RatePerHour { get; set; }
    public bool IsActive { get; set; } = true;
    public string TempPassword { get; set; } = "";
}