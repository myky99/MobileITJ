using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(Report), "Report")]
    public class ReportDetailsViewModel : BaseViewModel
    {
        private WorkerReport _report;

        public WorkerReport Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }

        public ReportDetailsViewModel()
        {
            // Constructor
        }
    }
}