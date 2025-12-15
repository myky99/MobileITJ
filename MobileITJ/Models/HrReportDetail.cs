using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MobileITJ.Models
{
    public class HrReportDetail : INotifyPropertyChanged
    {
        public WorkerDetail Worker { get; set; }
        public List<WorkerReport> Reports { get; set; } = new List<WorkerReport>();

        // 👇 CONTROLS VISIBILITY 👇
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(); // Updates UI immediately
                }
            }
        }

        public bool HasReports => Reports != null && Reports.Count > 0;

        public string ReportCountDisplay
        {
            get
            {
                if (Reports == null || Reports.Count == 0)
                    return "No reports filed (Good Standing)";

                return $"{Reports.Count} Incident(s) Reported (Tap to view)";
            }
        }

        // Standard INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}