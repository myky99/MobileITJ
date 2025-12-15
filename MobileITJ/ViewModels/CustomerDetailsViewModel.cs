using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MobileITJ.Models;
using MobileITJ.Services;
using System.Collections.Generic;
using System.Linq;

namespace MobileITJ.ViewModels
{
    [QueryProperty(nameof(Customer), "Customer")]
    public class CustomerDetailsViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _auth;
        private User _customer;

        public User Customer
        {
            get => _customer;
            set
            {
                SetProperty(ref _customer, value);
                if (value != null)
                {
                    Task.Run(async () => await LoadCustomerHistoryAsync());
                }
            }
        }

        // This list holds the Jobs + The Workers inside them
        public ObservableCollection<CustomerJobHistory> JobHistory { get; } = new ObservableCollection<CustomerJobHistory>();

        public CustomerDetailsViewModel(IAuthenticationService auth)
        {
            _auth = auth;
        }

        private async Task LoadCustomerHistoryAsync()
        {
            if (Customer == null) return;
            IsBusy = true;

            try
            {
                JobHistory.Clear();

                // 1. Get all jobs created by this customer
                var jobs = await _auth.GetJobsByCustomerIdAsync(Customer.Id);

                // 2. For each job, get the applications (workers)
                foreach (var job in jobs)
                {
                    var historyItem = new CustomerJobHistory
                    {
                        Job = job,
                        Workers = new List<WorkerStatusSummary>()
                    };

                    // Get applications for this specific job
                    var apps = await _auth.GetApplicationsForJobAsync(job.Id);

                    foreach (var app in apps)
                    {
                        string workStatus = "Pending";
                        if (app.Status == ApplicationStatus.Accepted)
                        {
                            if (app.JobStatus == JobStatus.Completed) workStatus = "Work Done";
                            else if (app.IsClockedIn) workStatus = "Working Now";
                            else if (app.TotalTimeSpent.TotalMinutes > 0) workStatus = "In Progress";
                            else workStatus = "Hired (Not Started)";
                        }
                        else if (app.Status == ApplicationStatus.Rejected) workStatus = "Rejected";

                        historyItem.Workers.Add(new WorkerStatusSummary
                        {
                            WorkerName = app.WorkerName,
                            ApplicationStatus = app.Status.ToString(),
                            WorkStatus = workStatus,
                            PaymentStatus = app.IsPaid ? "PAID" : "UNPAID",
                            IsPaid = app.IsPaid
                        });
                    }

                    JobHistory.Add(historyItem);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    // --- Helper Classes for UI Display ---
    public class CustomerJobHistory
    {
        public Job Job { get; set; }
        public List<WorkerStatusSummary> Workers { get; set; }
        public bool HasWorkers => Workers != null && Workers.Count > 0;
    }

    public class WorkerStatusSummary
    {
        public string WorkerName { get; set; }
        public string ApplicationStatus { get; set; }
        public string WorkStatus { get; set; }     // e.g., "Work Done", "Working Now"
        public string PaymentStatus { get; set; }  // "PAID" or "UNPAID"
        public bool IsPaid { get; set; }
    }
}