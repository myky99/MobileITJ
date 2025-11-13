using System.Collections.Generic;

namespace MobileITJ.Models
{
    public class HrReportDetail
    {
        public WorkerDetail Worker { get; set; }
        public List<WorkerReport> Reports { get; set; } = new List<WorkerReport>();

        // Helper property for the UI
        public bool HasReports => Reports.Any();
        public string ReportCountDisplay => HasReports ? $"{Reports.Count} Report(s)" : "No Reports";
    }
}