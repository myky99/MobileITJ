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
        private readonly string _userFilePath;
        private Dictionary<string, User> _users;
        private User? _currentUser;
        private bool _isInitialized = false;
        private readonly string _profileFilePath;
        private Dictionary<int, WorkerProfile> _workerProfiles;
        private readonly string _jobsFilePath;
        private List<Job> _jobs;
        private readonly string _applicationsFilePath;
        private List<JobApplication> _jobApplications;
        private readonly string _transactionsFilePath;
        private List<Transaction> _transactions;
        private readonly string _workerReportsFilePath;
        private List<WorkerReport> _workerReports;

        public AuthenticationService()
        {
            _userFilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");
            _users = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
            _profileFilePath = Path.Combine(FileSystem.AppDataDirectory, "worker_profiles.json");
            _workerProfiles = new Dictionary<int, WorkerProfile>();
            _jobsFilePath = Path.Combine(FileSystem.AppDataDirectory, "jobs.json");
            _jobs = new List<Job>();
            _applicationsFilePath = Path.Combine(FileSystem.AppDataDirectory, "job_applications.json");
            _jobApplications = new List<JobApplication>();
            _transactionsFilePath = Path.Combine(FileSystem.AppDataDirectory, "transactions.json");
            _transactions = new List<Transaction>();
            _workerReportsFilePath = Path.Combine(FileSystem.AppDataDirectory, "worker_reports.json");
            _workerReports = new List<WorkerReport>();
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized) return;
            await LoadUsersAsync();
            await LoadWorkerProfilesAsync();
            await LoadJobsAsync();
            await LoadWorkerReportsAsync();
            await LoadJobApplicationsAsync();
            await LoadTransactionsAsync();
            await SeedHrAdminAsync();
            _isInitialized = true;
        }

        private async Task SeedHrAdminAsync()
        {
            string hrEmail = "hr@itj.com";
            if (!_users.ContainsKey(hrEmail))
            {
                var hrUser = new User { Id = _users.Count + 1, FirstName = "Admin", LastName = "HR", Email = hrEmail, Password = "admin123", UserType = UserType.HR };
                _users[hrEmail] = hrUser;
                await SaveUsersAsync();
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string firstName, string lastName, string email, string password, UserType userType)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password required.");
            if (_users.ContainsKey(email))
                return (false, "Email already registered.");
            var newUser = new User { Id = _users.Count + 1, FirstName = firstName, LastName = lastName, Email = email, Password = password, UserType = userType };
            _users[email] = newUser;
            await SaveUsersAsync();
            return (true, "Registration successful!");
        }

        public async Task<(bool Success, string Message, string WorkerId, string TempPassword)> CreateWorkerAsync(string firstName, string lastName, string email, List<string> skills, decimal ratePerHour)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", "", "");
            if (_users.ContainsKey(email))
                return (false, "Email already registered.", "", "");
            string workerId = $"WKR-{_users.Count + 1:000}";
            string tempPassword = "temp123";
            var (regSuccess, regMessage) = await RegisterAsync(firstName, lastName, email, tempPassword, UserType.Worker);
            if (!regSuccess)
                return (false, regMessage, "", "");
            var newUser = _users[email];
            if (newUser == null)
                return (false, "Failed to retrieve new user after creation.", "", "");
            var newProfile = new WorkerProfile { Id = _workerProfiles.Count + 1, UserId = newUser.Id, WorkerId = workerId, TempPassword = tempPassword, Skills = skills, RatePerHour = ratePerHour, IsActive = true };
            _workerProfiles[newUser.Id] = newProfile;
            await SaveWorkerProfilesAsync();
            return (true, "Worker created successfully!", workerId, tempPassword);
        }

        public async Task<List<WorkerDetail>> GetAllWorkersAsync()
        {
            var workerDetails = new List<WorkerDetail>();
            var workerUsers = _users.Values.Where(u => u.UserType == UserType.Worker);
            foreach (var user in workerUsers)
            {
                if (_workerProfiles.TryGetValue(user.Id, out var profile))
                {
                    workerDetails.Add(new WorkerDetail
                    {
                        UserId = user.Id,
                        WorkerId = profile.WorkerId,
                        FirstName = user.FirstName ?? "N/A",
                        LastName = user.LastName ?? "N/A",
                        Email = user.Email ?? "N/A",
                        Skills = new List<string>(profile.Skills),
                        RatePerHour = profile.RatePerHour,
                        IsActive = profile.IsActive
                    });
                }
            }
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

        public async Task<List<User>> GetAllCustomersAsync()
        {
            var customerUsers = _users.Values
                .Where(u => u.UserType == UserType.Customer)
                .ToList();
            return await Task.FromResult(customerUsers);
        }

        public async Task<(bool Success, string Message)> CreateJobAsync(Job newJob)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return (false, "You must be logged in to create a job.");
            if (user.UserType != UserType.Customer)
                return (false, "Only customers can create jobs.");
            newJob.Id = _jobs.Count + 1;
            newJob.CustomerId = user.Id;
            newJob.DatePosted = DateTime.UtcNow;
            newJob.Status = JobStatus.Open;
            _jobs.Add(newJob);
            await SaveJobsAsync();
            return (true, "Job successfully posted!");
        }

        public async Task<List<CustomerJobDetail>> GetMyCustomerJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<CustomerJobDetail>();

            var myJobs = _jobs
                .Where(j => j.CustomerId == user.Id)
                .OrderByDescending(j => j.DatePosted)
                .ToList();

            var jobDetails = new List<CustomerJobDetail>();
            foreach (var job in myJobs)
            {
                var acceptedCount = _jobApplications.Count(app => app.JobId == job.Id && app.Status == ApplicationStatus.Accepted);

                jobDetails.Add(new CustomerJobDetail
                {
                    Job = job,
                    Status = job.Status,
                    SlotsDisplay = $"{acceptedCount} / {job.WorkersNeeded} workers",
                    CanCompleteJob = (job.Status == JobStatus.Ongoing && acceptedCount > 0 && !_jobApplications.Any(app => app.JobId == job.Id && app.ClockInTime != null))
                });
            }

            return await Task.FromResult(jobDetails);
        }

        public async Task<List<Job>> GetMyJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<Job>();
            var myJobs = _jobs
                .Where(j => j.CustomerId == user.Id)
                .OrderByDescending(j => j.DatePosted)
                .ToList();
            return await Task.FromResult(myJobs);
        }

        public async Task RateWorkerOnJobAsync(int applicationId, int rating, string review)
        {
            var appToUpdate = _jobApplications.FirstOrDefault(app => app.Id == applicationId);
            if (appToUpdate != null)
            {
                appToUpdate.IsRated = true;
                appToUpdate.Rating = rating;
                appToUpdate.Review = review;
                await SaveJobApplicationsAsync();
            }
        }

        public async Task<(bool Success, string Message)> PayWorkerAsync(int applicationId)
        {
            var appToUpdate = _jobApplications.FirstOrDefault(app => app.Id == applicationId);
            if (appToUpdate == null)
                return (false, "Application not found.");

            if (appToUpdate.IsPaid)
                return (false, "This worker has already been paid.");

            var job = _jobs.FirstOrDefault(j => j.Id == appToUpdate.JobId);
            if (job == null)
                return (false, "Job not found.");

            appToUpdate.IsPaid = true;

            var transaction = new Transaction
            {
                Id = _transactions.Count + 1,
                WorkerUserId = appToUpdate.WorkerUserId,
                JobId = appToUpdate.JobId,
                JobDescription = job.JobDescription,
                AmountPaid = (decimal)appToUpdate.TotalTimeSpent.TotalHours * appToUpdate.NegotiatedRate,
                DatePaid = DateTime.UtcNow
            };
            _transactions.Add(transaction);

            await SaveJobApplicationsAsync();
            await SaveTransactionsAsync();

            return (true, "Payment successful!");
        }

        public async Task<List<Job>> GetAvailableJobsAsync()
        {
            var openJobs = _jobs
                .Where(j => j.Status == JobStatus.Open)
                .OrderByDescending(j => j.DatePosted)
                .ToList();
            return await Task.FromResult(openJobs);
        }

        public async Task<(bool Success, string Message)> FileWorkerReportAsync(WorkerDetail worker, string message)
        {
            var user = await GetCurrentUserAsync();
            if (user == null || worker == null)
                return (false, "Error filing report.");

            var newReport = new WorkerReport
            {
                Id = _workerReports.Count + 1,
                WorkerUserId = worker.UserId,
                WorkerName = worker.FullName,
                CustomerId = user.Id,
                ReportMessage = message,
                DateFiled = DateTime.UtcNow
            };

            _workerReports.Add(newReport);
            await SaveWorkerReportsAsync();

            return (true, "Report filed successfully.");
        }

        public async Task<List<WorkerReport>> GetMyFiledWorkerReportsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<WorkerReport>();

            var myReports = _workerReports
                .Where(r => r.CustomerId == user.Id)
                .OrderByDescending(r => r.DateFiled)
                .ToList();

            return await Task.FromResult(myReports);
        }

        public async Task<(bool Success, string Message)> ApplyForJobAsync(int jobId, decimal negotiatedRate)
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return (false, "Only logged-in workers can apply.");

            bool alreadyApplied = _jobApplications.Any(app => app.JobId == jobId && app.WorkerUserId == user.Id);
            if (alreadyApplied)
            {
                return (false, "You have already applied for this job.");
            }

            var newApplication = new JobApplication
            {
                Id = _jobApplications.Count + 1,
                JobId = jobId,
                WorkerUserId = user.Id,
                NegotiatedRate = negotiatedRate,
                Status = ApplicationStatus.Pending,
                DateApplied = DateTime.UtcNow
            };

            _jobApplications.Add(newApplication);
            await SaveJobApplicationsAsync();

            return (true, "Application submitted successfully!");
        }

        public async Task<List<MyJobDetail>> GetMyWorkerJobsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return new List<MyJobDetail>();

            var jobsDict = _jobs.ToDictionary(j => j.Id);

            var myJobDetails = _jobApplications
                .Where(app => app.WorkerUserId == user.Id && app.Status == ApplicationStatus.Accepted)
                .Select(app => new MyJobDetail
                {
                    ApplicationId = app.Id,
                    Job = jobsDict.ContainsKey(app.JobId) ? jobsDict[app.JobId] : null,
                    TotalTimeSpent = app.TotalTimeSpent,
                    IsClockedIn = app.ClockInTime != null
                })
                .Where(jd => jd.Job != null)
                .OrderByDescending(jd => jd.Job.DatePosted)
                .ToList();

            return await Task.FromResult(myJobDetails);
        }

        public async Task<WorkerDetail?> GetMyWorkerProfileAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return null;

            if (_workerProfiles.TryGetValue(user.Id, out var profile))
            {
                return new WorkerDetail
                {
                    UserId = user.Id,
                    WorkerId = profile.WorkerId,
                    FirstName = user.FirstName ?? "N/A",
                    LastName = user.LastName ?? "N/A",
                    Email = user.Email ?? "N/A",
                    Skills = new List<string>(profile.Skills),
                    RatePerHour = profile.RatePerHour,
                    IsActive = profile.IsActive
                };
            }

            return null;
        }

        public async Task<List<RatingDetail>> GetMyRatingsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return new List<RatingDetail>();

            var jobsDict = _jobs.ToDictionary(j => j.Id);

            var myRatings = _jobApplications
                .Where(app => app.WorkerUserId == user.Id && app.IsRated)
                .OrderByDescending(app => app.DateApplied)
                .Select(app => new RatingDetail
                {
                    JobDescription = jobsDict.ContainsKey(app.JobId) ? jobsDict[app.JobId].JobDescription : "Unknown Job",
                    Rating = app.Rating,
                    Review = app.Review
                })
                .ToList();

            return await Task.FromResult(myRatings);
        }

        public async Task<List<JobApplicationDetail>> GetApplicationsForJobAsync(int jobId)
        {
            var applicationDetails = new List<JobApplicationDetail>();
            var applications = _jobApplications.Where(app => app.JobId == jobId);
            var workerUsers = _users.Values
                .Where(u => u.UserType == UserType.Worker)
                .ToDictionary(u => u.Id);

            var job = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (job == null) return applicationDetails;

            foreach (var app in applications)
            {
                if (workerUsers.TryGetValue(app.WorkerUserId, out var workerUser))
                {
                    applicationDetails.Add(new JobApplicationDetail
                    {
                        ApplicationId = app.Id,
                        JobId = app.JobId,
                        WorkerUserId = app.WorkerUserId,
                        WorkerName = $"{workerUser.FirstName} {workerUser.LastName}",
                        NegotiatedRate = app.NegotiatedRate,
                        Status = app.Status,
                        IsRated = app.IsRated,
                        Rating = app.Rating,
                        Review = app.Review,
                        TotalTimeSpent = app.TotalTimeSpent,
                        IsPaid = app.IsPaid,
                        IsClockedIn = app.ClockInTime != null,
                        JobStatus = job.Status
                    });
                }
            }
            return await Task.FromResult(applicationDetails);
        }

        public async Task<(bool Success, string Message)> AcceptApplicationAsync(JobApplicationDetail application)
        {
            var appToUpdate = _jobApplications.FirstOrDefault(app => app.Id == application.ApplicationId);
            if (appToUpdate == null)
                return (false, "Application not found.");

            var jobToUpdate = _jobs.FirstOrDefault(j => j.Id == application.JobId);
            if (jobToUpdate == null)
                return (false, "Job not found.");

            var acceptedCount = _jobApplications.Count(app => app.JobId == application.JobId && app.Status == ApplicationStatus.Accepted);

            if (acceptedCount >= jobToUpdate.WorkersNeeded)
            {
                return (false, "This job has already filled all its worker slots.");
            }

            appToUpdate.Status = ApplicationStatus.Accepted;

            if (acceptedCount + 1 >= jobToUpdate.WorkersNeeded)
            {
                jobToUpdate.Status = JobStatus.Ongoing;

                var otherApplications = _jobApplications
                    .Where(app => app.JobId == application.JobId && app.Status == ApplicationStatus.Pending);

                foreach (var otherApp in otherApplications)
                {
                    otherApp.Status = ApplicationStatus.Rejected;
                }
            }

            await SaveJobApplicationsAsync();
            await SaveJobsAsync();

            return (true, "Worker has been accepted for the job!");
        }

        public async Task<List<JobWithWorkerDetail>> GetMyJobsWithWorkerAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Customer)
                return new List<JobWithWorkerDetail>();

            var myJobs = _jobs.Where(j => j.CustomerId == user.Id);
            var details = new List<JobWithWorkerDetail>();
            var workerUsers = _users.Values.Where(u => u.UserType == UserType.Worker).ToDictionary(u => u.Id);

            foreach (var job in myJobs)
            {
                string workerName = "No worker assigned yet";
                var acceptedApp = _jobApplications.FirstOrDefault(app => app.JobId == job.Id && app.Status == ApplicationStatus.Accepted);

                if (acceptedApp != null)
                {
                    if (workerUsers.TryGetValue(acceptedApp.WorkerUserId, out var workerUser))
                    {
                        workerName = $"{workerUser.FirstName} {workerUser.LastName}";
                    }
                }

                details.Add(new JobWithWorkerDetail
                {
                    Job = job,
                    WorkerName = workerName
                });
            }

            return await Task.FromResult(details);
        }

        public async Task<(bool Success, string Message)> ClockInAsync(int applicationId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return (false, "Only logged-in workers can clock in.");

            var anyClockedIn = _jobApplications.FirstOrDefault(app => app.WorkerUserId == user.Id && app.ClockInTime != null);
            if (anyClockedIn != null)
                return (false, "You are already clocked in to another job.");

            var appToUpdate = _jobApplications.FirstOrDefault(app => app.Id == applicationId);
            if (appToUpdate == null)
                return (false, "Application not found.");

            appToUpdate.ClockInTime = DateTime.UtcNow;
            await SaveJobApplicationsAsync();
            return (true, "Clocked in!");
        }

        public async Task<(bool Success, string Message, TimeSpan? TotalTime)> ClockOutAsync(int applicationId)
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return (false, "Only logged-in workers can clock out.", null);

            var appToUpdate = _jobApplications.FirstOrDefault(app => app.Id == applicationId);
            if (appToUpdate == null)
                return (false, "Application not found.", null);

            if (appToUpdate.ClockInTime == null)
                return (false, "You are not clocked in to this job.", null);

            TimeSpan sessionTime = DateTime.UtcNow - appToUpdate.ClockInTime.Value;
            appToUpdate.TotalTimeSpent += sessionTime;
            appToUpdate.ClockInTime = null;

            await SaveJobApplicationsAsync();
            return (true, "Clocked out!", appToUpdate.TotalTimeSpent);
        }

        public async Task<(bool Success, string Message)> CustomerCompleteJobAsync(int jobId)
        {
            var jobToUpdate = _jobs.FirstOrDefault(j => j.Id == jobId);
            if (jobToUpdate == null)
                return (false, "Job not found.");

            if (jobToUpdate.Status == JobStatus.Completed)
                return (false, "This job is already marked as complete.");

            var acceptedApps = _jobApplications
                .Where(app => app.JobId == jobId && app.Status == ApplicationStatus.Accepted);

            if (!acceptedApps.Any())
                return (false, "There are no workers assigned to this job yet.");

            var clockedInWorker = acceptedApps.FirstOrDefault(app => app.ClockInTime != null);
            if (clockedInWorker != null)
            {
                var workerUser = _users.Values.FirstOrDefault(u => u.Id == clockedInWorker.WorkerUserId);
                string workerName = workerUser?.FirstName ?? "A worker";
                return (false, $"{workerName} is still clocked in. All workers must clock out before you can complete the job.");
            }

            jobToUpdate.Status = JobStatus.Completed;
            await SaveJobsAsync();

            return (true, "Job has been successfully marked as complete! You can now rate the workers.");
        }

        public async Task<List<Transaction>> GetMyTransactionsAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || user.UserType != UserType.Worker)
                return new List<Transaction>();

            var myTransactions = _transactions
                .Where(t => t.WorkerUserId == user.Id)
                .OrderByDescending(t => t.DatePaid)
                .ToList();

            return await Task.FromResult(myTransactions);
        }

        // --- 👇 NEW METHOD: GetAllWorkerReportsAsync 👇 ---
        public async Task<List<WorkerReport>> GetAllWorkerReportsAsync()
        {
            // Just return all of them, ordered by most recent
            return await Task.FromResult(_workerReports.OrderByDescending(r => r.DateFiled).ToList());
        }
        // --- END OF NEW ---

        public async Task<(bool Success, string Message, UserType? UserType, bool IsFirstLogin)> LoginAsync(string email, string password)
        {
            if (!_users.TryGetValue(email, out var user))
                return (false, "User not found.", null, false);

            if (user.Password != password)
                return (false, "Invalid password.", null, false);

            if (user.UserType == UserType.Worker)
            {
                if (_workerProfiles.TryGetValue(user.Id, out var profile) && !profile.IsActive)
                {
                    return (false, "Account deactivated.", null, false);
                }
            }

            _currentUser = user;
            await SecureStorage.Default.SetAsync("user_token", Guid.NewGuid().ToString());

            bool isFirstLogin = false;
            if (user.UserType == UserType.Worker)
            {
                if (_workerProfiles.TryGetValue(user.Id, out var profile))
                {
                    if (profile.TempPassword == password)
                    {
                        isFirstLogin = true;
                        profile.TempPassword = string.Empty;
                        await SaveWorkerProfilesAsync();
                    }
                }
            }

            return (true, "Login successful!", user.UserType, isFirstLogin);
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            foreach (var kvp in _users)
            {
                if (kvp.Value.Id == userId)
                {
                    if (kvp.Value.Password != currentPassword)
                        return (false, "Current password is incorrect.");

                    kvp.Value.Password = newPassword;
                    await SaveUsersAsync();

                    if (kvp.Value.UserType == UserType.Worker && _workerProfiles.TryGetValue(userId, out var profile))
                    {
                        profile.TempPassword = string.Empty;
                        await SaveWorkerProfilesAsync();
                    }

                    return (true, "Password changed successfully!");
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

        private async Task SaveUsersAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_users);
                await File.WriteAllTextAsync(_userFilePath, json);
            }
            catch (Exception ex) { Console.WriteLine($"Error saving users: {ex.Message}"); }
        }
        private async Task LoadUsersAsync()
        {
            try
            {
                if (File.Exists(_userFilePath))
                {
                    var json = await File.ReadAllTextAsync(_userFilePath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, User>>(json);
                    if (data != null) _users = data;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading users: {ex.Message}"); }
        }
        private async Task SaveWorkerProfilesAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_workerProfiles);
                await File.WriteAllTextAsync(_profileFilePath, json);
            }
            catch (Exception ex) { Console.WriteLine($"Error saving worker profiles: {ex.Message}"); }
        }
        private async Task LoadWorkerProfilesAsync()
        {
            try
            {
                if (File.Exists(_profileFilePath))
                {
                    var json = await File.ReadAllTextAsync(_profileFilePath);
                    var data = JsonSerializer.Deserialize<Dictionary<int, WorkerProfile>>(json);
                    if (data != null) _workerProfiles = data;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading worker profiles: {ex.Message}"); }
        }
        private async Task SaveJobsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_jobs);
                await File.WriteAllTextAsync(_jobsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving jobs: {ex.Message}");
            }
        }
        private async Task LoadJobsAsync()
        {
            try
            {
                if (File.Exists(_jobsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_jobsFilePath);
                    var data = JsonSerializer.Deserialize<List<Job>>(json);
                    if (data != null)
                        _jobs = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading jobs: {ex.Message}");
            }
        }

        private async Task SaveJobApplicationsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_jobApplications);
                await File.WriteAllTextAsync(_applicationsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving job applications: {ex.Message}");
            }
        }

        private async Task LoadJobApplicationsAsync()
        {
            try
            {
                if (File.Exists(_applicationsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_applicationsFilePath);
                    var data = JsonSerializer.Deserialize<List<JobApplication>>(json);
                    if (data != null)
                        _jobApplications = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading job applications: {ex.Message}");
            }
        }

        private async Task SaveTransactionsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_transactions);
                await File.WriteAllTextAsync(_transactionsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transactions: {ex.Message}");
            }
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                if (File.Exists(_transactionsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_transactionsFilePath);
                    var data = JsonSerializer.Deserialize<List<Transaction>>(json);
                    if (data != null)
                        _transactions = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transactions: {ex.Message}");
            }
        }

        private async Task SaveWorkerReportsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_workerReports);
                await File.WriteAllTextAsync(_workerReportsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving worker reports: {ex.Message}");
            }
        }

        private async Task LoadWorkerReportsAsync()
        {
            try
            {
                if (File.Exists(_workerReportsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_workerReportsFilePath);
                    var data = JsonSerializer.Deserialize<List<WorkerReport>>(json);
                    if (data != null)
                        _workerReports = data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading worker reports: {ex.Message}");
            }
        }
    }
}