using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using MobileITJ.Models;
using System.Linq;

namespace MobileITJ.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // File Paths
        private readonly string _userFilePath;
        private readonly string _profileFilePath;
        private readonly string _jobsFilePath;
        private readonly string _workerReportsFilePath;
        private readonly string _applicationsFilePath;
        private readonly string _transactionsFilePath;
        private readonly string _skillCategoriesFilePath;

        // Data Storage
        private Dictionary<string, User> _users;
        private Dictionary<int, WorkerProfile> _workerProfiles;
        private List<Job> _jobs;
        private List<WorkerReport> _workerReports;
        private List<JobApplication> _jobApplications;
        private List<Transaction> _transactions;
        private List<string> _skillCategories;

        private User? _currentUser;
        private bool _isInitialized = false;

        public AuthenticationService()
        {
            // 1. Setup File Paths
            _userFilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");
            _profileFilePath = Path.Combine(FileSystem.AppDataDirectory, "worker_profiles.json");
            _jobsFilePath = Path.Combine(FileSystem.AppDataDirectory, "jobs.json");
            _workerReportsFilePath = Path.Combine(FileSystem.AppDataDirectory, "worker_reports.json");
            _applicationsFilePath = Path.Combine(FileSystem.AppDataDirectory, "job_applications.json");
            _transactionsFilePath = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");
            _skillCategoriesFilePath = Path.Combine(FileSystem.AppDataDirectory, "skill_categories.json");

            // 2. Initialize Empty Collections immediately
            _users = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
            _workerProfiles = new Dictionary<int, WorkerProfile>();
            _jobs = new List<Job>();
            _workerReports = new List<WorkerReport>();
            _jobApplications = new List<JobApplication>();
            _transactions = new List<Transaction>();
            _skillCategories = new List<string>();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            // 3. Load all data asynchronously
            await LoadUsersAsync();
            await LoadWorkerProfilesAsync();
            await LoadJobsAsync();
            await LoadWorkerReportsAsync();
            await LoadJobApplicationsAsync();
            await LoadTransactionsAsync();
            await LoadSkillCategoriesAsync();

            await SeedHrAdminAsync();
            _isInitialized = true;
        }

        private async Task SeedHrAdminAsync()
        {
            string hrEmail = "hr@itj.com";
            if (!_users.ContainsKey(hrEmail))
            {
                var hrUser = new User
                {
                    Id = _users.Count + 1,
                    FirstName = "Admin",
                    LastName = "HR",
                    Email = hrEmail,
                    Password = "admin123",
                    UserType = UserType.HR
                };
                _users[hrEmail] = hrUser;
                await SaveUsersAsync();
            }
        }

        // -----------------------------------------------------------------------------
        // 👇 SKILL CATEGORY MANAGEMENT
        // -----------------------------------------------------------------------------
        public async Task<List<string>> GetSkillCategoriesAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            return new List<string>(_skillCategories);
        }

        public async Task AddSkillCategoryAsync(string category)
        {
            if (!_skillCategories.Contains(category))
            {
                int index = _skillCategories.IndexOf("Something Else");

                if (index >= 0)
                {
                    _skillCategories.Insert(index, category);
                }
                else
                {
                    _skillCategories.Add(category);
                }

                await SaveSkillCategoriesAsync();
            }
        }

        public async Task RemoveSkillCategoryAsync(string category)
        {
            if (_skillCategories.Contains(category))
            {
                _skillCategories.Remove(category);
                await SaveSkillCategoriesAsync();
            }
        }

        private async Task LoadSkillCategoriesAsync()
        {
            try
            {
                if (File.Exists(_skillCategoriesFilePath))
                {
                    var json = await File.ReadAllTextAsync(_skillCategoriesFilePath);
                    _skillCategories = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }

                // Set Defaults if empty
                if (_skillCategories.Count == 0)
                {
                    _skillCategories = new List<string>
                    {
                        "Plumbing", "Mason", "Gardening", "Welding", "Programming", "Something Else"
                    };
                    await SaveSkillCategoriesAsync();
                }
            }
            catch { }
        }

        private async Task SaveSkillCategoriesAsync()
        {
            try { await File.WriteAllTextAsync(_skillCategoriesFilePath, JsonSerializer.Serialize(_skillCategories)); } catch { }
        }
        // -----------------------------------------------------------------------------

        // ... (User & Auth Methods) ...

        public async Task<(bool Success, string Message)> RegisterAsync(string firstName, string lastName, string email, string password, UserType userType)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return (false, "Email and password required.");
            if (_users.ContainsKey(email)) return (false, "Email already registered.");

            var newUser = new User { Id = _users.Count + 1, FirstName = firstName, LastName = lastName, Email = email, Password = password, UserType = userType };
            _users[email] = newUser;
            await SaveUsersAsync();
            return (true, "Registration successful!");
        }

        public async Task<(bool Success, string Message, UserType? UserType, bool IsFirstLogin)> LoginAsync(string email, string password)
        {
            if (!_users.TryGetValue(email, out var user)) return (false, "User not found.", null, false);
            if (user.Password != password) return (false, "Invalid password.", null, false);
            if (user.UserType == UserType.Worker && _workerProfiles.TryGetValue(user.Id, out var profile) && !profile.IsActive) return (false, "Account deactivated.", null, false);

            _currentUser = user;
            await SecureStorage.Default.SetAsync("user_token", Guid.NewGuid().ToString());

            bool isFirstLogin = false;
            if (user.UserType == UserType.Worker && _workerProfiles.TryGetValue(user.Id, out var wProfile) && wProfile.TempPassword == password)
            {
                isFirstLogin = true;
                wProfile.TempPassword = string.Empty;
                await SaveWorkerProfilesAsync();
            }
            return (true, "Login successful!", user.UserType, isFirstLogin);
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            foreach (var kvp in _users)
            {
                if (kvp.Value.Id == userId)
                {
                    if (kvp.Value.Password != currentPassword) return (false, "Incorrect password.");
                    kvp.Value.Password = newPassword;
                    await SaveUsersAsync();
                    if (kvp.Value.UserType == UserType.Worker && _workerProfiles.TryGetValue(userId, out var profile))
                    {
                        profile.TempPassword = string.Empty;
                        await SaveWorkerProfilesAsync();
                    }
                    return (true, "Password changed.");
                }
            }
            return (false, "User not found.");
        }

        public async Task<User?> GetCurrentUserAsync() => await Task.FromResult(_currentUser);

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.Default.GetAsync("user_token");
            return !string.IsNullOrEmpty(token) && _currentUser != null;
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            SecureStorage.Default.Remove("user_token");
            await Task.CompletedTask;
        }

        public async Task<(bool Success, string Message, string WorkerId, string TempPassword)> CreateWorkerAsync(string firstName, string lastName, string email, List<string> skills, decimal ratePerHour)
        {
            string workerId = $"WKR-{_users.Count + 1:000}";
            string tempPassword = "temp123";
            var (regSuccess, regMessage) = await RegisterAsync(firstName, lastName, email, tempPassword, UserType.Worker);
            if (!regSuccess) return (false, regMessage, "", "");

            var newUser = _users[email];
            var newProfile = new WorkerProfile { Id = _workerProfiles.Count + 1, UserId = newUser.Id, WorkerId = workerId, TempPassword = tempPassword, Skills = skills, RatePerHour = ratePerHour, IsActive = true };
            _workerProfiles[newUser.Id] = newProfile;
            await SaveWorkerProfilesAsync();
            return (true, "Worker created!", workerId, tempPassword);
        }

        public async Task<List<WorkerDetail>> GetAllWorkersAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            var workerDetails = new List<WorkerDetail>();
            var workerUsers = _users.Values.Where(u => u.UserType == UserType.Worker);
            bool dataWasRepaired = false;

            foreach (var user in workerUsers)
            {
                if (!_workerProfiles.TryGetValue(user.Id, out var profile))
                {
                    profile = new WorkerProfile { Id = _workerProfiles.Count + 1, UserId = user.Id, WorkerId = $"WKR-{user.Id:000}", IsActive = true, RatePerHour = 0, Skills = new List<string> { "Unspecified" }, TempPassword = "" };
                    _workerProfiles[user.Id] = profile;
                    dataWasRepaired = true;
                }
                workerDetails.Add(new WorkerDetail { UserId = user.Id, WorkerId = profile.WorkerId, FirstName = user.FirstName ?? "N/A", LastName = user.LastName ?? "N/A", Email = user.Email ?? "N/A", Skills = new List<string>(profile.Skills), RatePerHour = profile.RatePerHour, IsActive = profile.IsActive });
            }
            if (dataWasRepaired) await SaveWorkerProfilesAsync();
            return await Task.FromResult(workerDetails);
        }

        public async Task UpdateWorkerProfileAsync(WorkerDetail worker)
        {
            if (_workerProfiles.TryGetValue(worker.UserId, out var profileToUpdate))
            {
                profileToUpdate.IsActive = worker.IsActive;
                profileToUpdate.Skills = new List<string>(worker.Skills);
                profileToUpdate.RatePerHour = worker.RatePerHour;
                await SaveWorkerProfilesAsync();
            }
        }

        public async Task<WorkerDetail?> GetMyWorkerProfileAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker) return null;
            if (_workerProfiles.TryGetValue(user.Id, out var profile))
            {
                return new WorkerDetail { UserId = user.Id, WorkerId = profile.WorkerId, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Skills = new List<string>(profile.Skills), RatePerHour = profile.RatePerHour, IsActive = profile.IsActive };
            }
            return null;
        }

        public async Task<(bool Success, string Message)> CreateJobAsync(Job newJob)
        {
            var user = await GetCurrentUserAsync();
            newJob.Id = _jobs.Count + 1;
            newJob.CustomerId = user.Id;
            newJob.DatePosted = DateTime.UtcNow;
            newJob.Status = JobStatus.Open;
            _jobs.Add(newJob);
            await SaveJobsAsync();
            return (true, "Job posted!");
        }

        public async Task<List<CustomerJobDetail>> GetMyCustomerJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return new List<CustomerJobDetail>();
            var myJobs = _jobs.Where(j => j.CustomerId == user.Id).OrderByDescending(j => j.DatePosted).ToList();
            var list = new List<CustomerJobDetail>();
            foreach (var j in myJobs)
            {
                var accepted = _jobApplications.Count(a => a.JobId == j.Id && a.Status == ApplicationStatus.Accepted);
                list.Add(new CustomerJobDetail { Job = j, Status = j.Status, SlotsDisplay = $"{accepted}/{j.WorkersNeeded} workers", CanCompleteJob = j.Status == JobStatus.Ongoing && accepted > 0 });
            }
            return await Task.FromResult(list);
        }

        public async Task<List<Job>> GetMyJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            return await Task.FromResult(user == null ? new List<Job>() : _jobs.Where(j => j.CustomerId == user.Id).OrderByDescending(j => j.DatePosted).ToList());
        }

        public async Task<List<Job>> GetAvailableJobsAsync() => await Task.FromResult(_jobs.Where(j => j.Status == JobStatus.Open).OrderByDescending(j => j.DatePosted).ToList());

        public async Task<List<Job>> GetJobsByCustomerIdAsync(int customerId) => await Task.FromResult(_jobs.Where(j => j.CustomerId == customerId).OrderByDescending(j => j.DatePosted).ToList());

        // 👇 NEW: For HR Stats
        public async Task<List<Job>> GetAllJobsAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            // Return all jobs in the system
            return await Task.FromResult(_jobs);
        }
        // 👆 END NEW

        public async Task<(bool Success, string Message)> ApplyForJobAsync(int jobId, decimal negotiatedRate)
        {
            var user = await GetCurrentUserAsync();
            if (_jobApplications.Any(a => a.JobId == jobId && a.WorkerUserId == user.Id)) return (false, "Already applied.");
            _jobApplications.Add(new JobApplication { Id = _jobApplications.Count + 1, JobId = jobId, WorkerUserId = user.Id, NegotiatedRate = negotiatedRate, Status = ApplicationStatus.Pending, DateApplied = DateTime.UtcNow });
            await SaveJobApplicationsAsync();
            return (true, "Applied!");
        }

        public async Task<List<JobApplicationDetail>> GetApplicationsForJobAsync(int jobId)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            var details = new List<JobApplicationDetail>();

            // Get all applications for this job
            var currentJobApps = _jobApplications.Where(a => a.JobId == jobId);

            foreach (var app in currentJobApps)
            {
                var w = _users.Values.FirstOrDefault(u => u.Id == app.WorkerUserId);

                // 👇 NEW: Calculate Average Rating for this Worker
                // Find all *rated* applications for this specific worker across ALL jobs
                var workerRatings = _jobApplications
                    .Where(a => a.WorkerUserId == app.WorkerUserId && a.IsRated)
                    .Select(a => (double)a.Rating)
                    .ToList();

                double avgRating = workerRatings.Any() ? workerRatings.Average() : 0.0;

                details.Add(new JobApplicationDetail
                {
                    ApplicationId = app.Id,
                    JobId = app.JobId,
                    WorkerUserId = app.WorkerUserId,
                    WorkerName = w != null ? $"{w.FirstName} {w.LastName}" : "Unknown",
                    NegotiatedRate = app.NegotiatedRate,
                    Status = app.Status,
                    IsRated = app.IsRated,
                    Rating = app.Rating,
                    Review = app.Review,
                    TotalTimeSpent = app.TotalTimeSpent,
                    IsPaid = app.IsPaid,
                    IsClockedIn = app.ClockInTime != null,
                    JobStatus = job?.Status ?? JobStatus.Open,
                    AverageRating = avgRating
                });
            }
            return await Task.FromResult(details);
        }

        public async Task<(bool Success, string Message)> AcceptApplicationAsync(JobApplicationDetail application)
        {
            var app = _jobApplications.FirstOrDefault(a => a.Id == application.ApplicationId);
            var job = _jobs.FirstOrDefault(j => j.Id == application.JobId);
            if (app == null || job == null) return (false, "Error.");
            var count = _jobApplications.Count(a => a.JobId == job.Id && a.Status == ApplicationStatus.Accepted);
            if (count >= job.WorkersNeeded) return (false, "Full.");
            app.Status = ApplicationStatus.Accepted;
            if (count + 1 >= job.WorkersNeeded) { job.Status = JobStatus.Ongoing; foreach (var p in _jobApplications.Where(a => a.JobId == job.Id && a.Status == ApplicationStatus.Pending)) p.Status = ApplicationStatus.Rejected; }
            await SaveJobApplicationsAsync(); await SaveJobsAsync();
            return (true, "Accepted.");
        }

        // 👇 THIS IS THE MISSING METHOD YOU NEED
        public async Task<(bool Success, string Message)> RejectApplicationAsync(int applicationId)
        {
            var app = _jobApplications.FirstOrDefault(a => a.Id == applicationId);
            if (app == null) return (false, "Application not found.");

            app.Status = ApplicationStatus.Rejected;
            await SaveJobApplicationsAsync();
            return (true, "Application rejected.");
        }
        // 👆 END MISSING METHOD

        public async Task<List<MyJobDetail>> GetMyWorkerJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return new List<MyJobDetail>();
            var jobs = _jobs.ToDictionary(j => j.Id);

            return await Task.FromResult(_jobApplications
                .Where(a => a.WorkerUserId == user.Id &&
                           (a.Status == ApplicationStatus.Accepted || a.Status == ApplicationStatus.Pending))
                .Select(a => new MyJobDetail
                {
                    ApplicationId = a.Id,
                    Job = jobs.ContainsKey(a.JobId) ? jobs[a.JobId] : null,
                    TotalTimeSpent = a.TotalTimeSpent,
                    IsClockedIn = a.ClockInTime != null,
                    Status = a.Status
                })
                .Where(j => j.Job != null)
                .OrderByDescending(j => j.Job.DatePosted)
                .ToList());
        }

        public async Task<(bool Success, string Message)> ClockInAsync(int applicationId)
        {
            var user = await GetCurrentUserAsync();
            var app = _jobApplications.FirstOrDefault(a => a.Id == applicationId);
            if (_jobApplications.Any(a => a.WorkerUserId == user.Id && a.ClockInTime != null)) return (false, "Clocked in elsewhere.");
            app.ClockInTime = DateTime.UtcNow;
            await SaveJobApplicationsAsync();
            return (true, "Clocked In!");
        }

        public async Task<(bool Success, string Message, TimeSpan? TotalTime)> ClockOutAsync(int applicationId)
        {
            var app = _jobApplications.FirstOrDefault(a => a.Id == applicationId);
            var time = DateTime.UtcNow - app.ClockInTime.Value;
            app.TotalTimeSpent += time;
            app.ClockInTime = null;
            await SaveJobApplicationsAsync();
            return (true, "Clocked Out!", app.TotalTimeSpent);
        }

        public async Task<(bool Success, string Message)> CustomerCompleteJobAsync(int jobId)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (_jobApplications.Any(a => a.JobId == jobId && a.ClockInTime != null)) return (false, "Workers still clocked in.");

            job.Status = JobStatus.Completed;
            job.DateCompleted = DateTime.UtcNow;

            await SaveJobsAsync();
            return (true, "Completed!");
        }

        public async Task RateWorkerOnJobAsync(int applicationId, int rating, string review)
        {
            var app = _jobApplications.FirstOrDefault(a => a.Id == applicationId);
            if (app != null) { app.IsRated = true; app.Rating = rating; app.Review = review; await SaveJobApplicationsAsync(); }
        }

        public async Task<List<RatingDetail>> GetMyRatingsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return new List<RatingDetail>();

            var details = new List<RatingDetail>();
            var jobsDict = _jobs.ToDictionary(j => j.Id);
            var customersDict = _users.Values.ToDictionary(u => u.Id);

            var myRatedApps = _jobApplications
                .Where(a => a.WorkerUserId == user.Id && a.IsRated)
                .OrderByDescending(a => a.Id);

            foreach (var app in myRatedApps)
            {
                string jobDesc = "Unknown Job";
                string custName = "Unknown Client";

                if (jobsDict.TryGetValue(app.JobId, out var job))
                {
                    jobDesc = !string.IsNullOrEmpty(job.Title) ? job.Title : job.JobDescription;
                    if (customersDict.TryGetValue(job.CustomerId, out var customer))
                    {
                        custName = $"{customer.FirstName} {customer.LastName}";
                    }
                }

                details.Add(new RatingDetail
                {
                    JobDescription = jobDesc,
                    Rating = app.Rating,
                    Review = app.Review,
                    CustomerName = custName
                });
            }

            return await Task.FromResult(details);
        }

        public async Task<(bool Success, string Message)> PayWorkerAsync(int applicationId)
        {
            var app = _jobApplications.FirstOrDefault(a => a.Id == applicationId);
            var job = _jobs.FirstOrDefault(j => j.Id == app.JobId);
            if (app.IsPaid) return (false, "Already paid.");
            app.IsPaid = true;
            _transactions.Add(new Transaction { Id = _transactions.Count + 1, WorkerUserId = app.WorkerUserId, JobId = app.JobId, JobDescription = job?.JobDescription ?? "Job", AmountPaid = (decimal)app.TotalTimeSpent.TotalHours * app.NegotiatedRate, DatePaid = DateTime.UtcNow });
            await SaveJobApplicationsAsync(); await SaveTransactionsAsync();
            return (true, "Paid!");
        }

        public async Task<List<Transaction>> GetMyTransactionsAsync()
        {
            var user = await GetCurrentUserAsync();
            return await Task.FromResult(_transactions.Where(t => t.WorkerUserId == user.Id).OrderByDescending(t => t.DatePaid).ToList());
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            return await Task.FromResult(_transactions.OrderByDescending(t => t.DatePaid).ToList());
        }

        public async Task<(bool Success, string Message)> FileWorkerReportAsync(WorkerDetail worker, Job job, string message)
        {
            var user = await GetCurrentUserAsync();
            var newReport = new WorkerReport { Id = _workerReports.Count + 1, WorkerUserId = worker.UserId, WorkerName = worker.FullName, CustomerId = user.Id, CustomerName = $"{user.FirstName} {user.LastName}", CustomerEmail = user.Email, JobId = job.Id, JobDescription = job.JobDescription, ReportMessage = message, DateFiled = DateTime.UtcNow };
            _workerReports.Add(newReport);
            await SaveWorkerReportsAsync();
            return (true, "Report filed.");
        }

        public async Task<List<WorkerReport>> GetMyFiledWorkerReportsAsync()
        {
            var user = await GetCurrentUserAsync();
            return await Task.FromResult(_workerReports.Where(r => r.CustomerId == user.Id).OrderByDescending(r => r.DateFiled).ToList());
        }

        public async Task<List<WorkerReport>> GetAllWorkerReportsAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            return await Task.FromResult(_workerReports.OrderByDescending(r => r.DateFiled).ToList());
        }

        public async Task<List<JobWithWorkerDetail>> GetMyJobsWithWorkerAsync()
        {
            var user = await GetCurrentUserAsync();
            var list = new List<JobWithWorkerDetail>();
            foreach (var job in _jobs.Where(j => j.CustomerId == user.Id))
            {
                foreach (var app in _jobApplications.Where(a => a.JobId == job.Id && a.Status == ApplicationStatus.Accepted))
                {
                    var w = _users.Values.FirstOrDefault(u => u.Id == app.WorkerUserId);
                    list.Add(new JobWithWorkerDetail { Job = job, WorkerName = w != null ? $"{w.FirstName} {w.LastName}" : "Unknown" });
                }
            }
            return await Task.FromResult(list);
        }

        public async Task<List<JobApplicationDetail>> GetWorkerJobHistoryAsync(int workerUserId)
        {
            var history = new List<JobApplicationDetail>();
            var jobs = _jobs.ToDictionary(j => j.Id);
            foreach (var app in _jobApplications.Where(a => a.WorkerUserId == workerUserId))
            {
                if (jobs.TryGetValue(app.JobId, out var job))
                {
                    history.Add(new JobApplicationDetail { ApplicationId = app.Id, JobId = app.JobId, WorkerUserId = app.WorkerUserId, NegotiatedRate = app.NegotiatedRate, Status = app.Status, TotalTimeSpent = app.TotalTimeSpent, IsClockedIn = app.ClockInTime != null, IsPaid = app.IsPaid, IsRated = app.IsRated, Rating = app.Rating, Review = app.Review, WorkerName = job.JobDescription, JobStatus = job.Status });
                }
            }
            return await Task.FromResult(history.OrderByDescending(h => h.ApplicationId).ToList());
        }

        public async Task<(bool Success, string Message)> CustomerMarkJobIncompleteAsync(int jobId, string reason)
        {
            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (job == null) return (false, "Job not found.");

            // Check if it's already closed
            if (job.Status == JobStatus.Completed || job.Status == JobStatus.Incomplete)
                return (false, "Job is already closed.");

            job.Status = JobStatus.Incomplete;
            job.IncompleteReason = reason; // Save the reason

            await SaveJobsAsync();
            return (true, "Status updated.");
        }

        public async Task<List<User>> GetAllCustomersAsync()
        {
            if (!_isInitialized) await InitializeAsync();
            return await Task.FromResult(_users.Values.Where(u => u.UserType == UserType.Customer).ToList());
        }

        // Persistence Helpers
        private async Task SaveUsersAsync() { try { await File.WriteAllTextAsync(_userFilePath, JsonSerializer.Serialize(_users)); } catch { } }
        private async Task LoadUsersAsync() { try { if (File.Exists(_userFilePath)) _users = JsonSerializer.Deserialize<Dictionary<string, User>>(await File.ReadAllTextAsync(_userFilePath)) ?? new Dictionary<string, User>(); } catch { } }
        private async Task SaveWorkerProfilesAsync() { try { await File.WriteAllTextAsync(_profileFilePath, JsonSerializer.Serialize(_workerProfiles)); } catch { } }
        private async Task LoadWorkerProfilesAsync() { try { if (File.Exists(_profileFilePath)) { var json = await File.ReadAllTextAsync(_profileFilePath); _workerProfiles = JsonSerializer.Deserialize<Dictionary<int, WorkerProfile>>(json) ?? new Dictionary<int, WorkerProfile>(); } } catch { } }
        private async Task SaveJobsAsync() { try { await File.WriteAllTextAsync(_jobsFilePath, JsonSerializer.Serialize(_jobs)); } catch { } }
        private async Task LoadJobsAsync() { try { if (File.Exists(_jobsFilePath)) _jobs = JsonSerializer.Deserialize<List<Job>>(await File.ReadAllTextAsync(_jobsFilePath)) ?? new List<Job>(); } catch { } }
        private async Task SaveWorkerReportsAsync() { try { await File.WriteAllTextAsync(_workerReportsFilePath, JsonSerializer.Serialize(_workerReports)); } catch { } }
        private async Task LoadWorkerReportsAsync() { try { if (File.Exists(_workerReportsFilePath)) _workerReports = JsonSerializer.Deserialize<List<WorkerReport>>(await File.ReadAllTextAsync(_workerReportsFilePath)) ?? new List<WorkerReport>(); } catch { } }
        private async Task SaveJobApplicationsAsync() { try { await File.WriteAllTextAsync(_applicationsFilePath, JsonSerializer.Serialize(_jobApplications)); } catch { } }
        private async Task LoadJobApplicationsAsync() { try { if (File.Exists(_applicationsFilePath)) _jobApplications = JsonSerializer.Deserialize<List<JobApplication>>(await File.ReadAllTextAsync(_applicationsFilePath)) ?? new List<JobApplication>(); } catch { } }
        private async Task SaveTransactionsAsync() { try { await File.WriteAllTextAsync(_transactionsFilePath, JsonSerializer.Serialize(_transactions)); } catch { } }
        private async Task LoadTransactionsAsync() { try { if (File.Exists(_transactionsFilePath)) _transactions = JsonSerializer.Deserialize<List<Transaction>>(await File.ReadAllTextAsync(_transactionsFilePath)) ?? new List<Transaction>(); } catch { } }
    }
}