using System.Collections.Generic;
using System.Threading.Tasks;
using MobileITJ.Models;
using System;

namespace MobileITJ.Services
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();

        // Auth
        Task<(bool Success, string Message, UserType? UserType, bool IsFirstLogin)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> RegisterAsync(string firstName, string lastName, string email, string password, UserType userType);
        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        // Worker Management
        Task<(bool Success, string Message, string WorkerId, string TempPassword)> CreateWorkerAsync(string firstName, string lastName, string email, List<string> skills, decimal ratePerHour);
        Task<List<WorkerDetail>> GetAllWorkersAsync();
        Task UpdateWorkerProfileAsync(WorkerDetail worker);
        Task<WorkerDetail?> GetMyWorkerProfileAsync();
        Task<List<JobApplicationDetail>> GetWorkerJobHistoryAsync(int workerUserId);

        // Skill Categories Management
        Task<List<string>> GetSkillCategoriesAsync();
        Task AddSkillCategoryAsync(string category);
        Task RemoveSkillCategoryAsync(string category);

        // Job Management
        Task<(bool Success, string Message)> CreateJobAsync(Job newJob);
        Task<List<CustomerJobDetail>> GetMyCustomerJobsAsync();
        Task<List<Job>> GetMyJobsAsync();
        Task<List<Job>> GetAvailableJobsAsync();
        Task<List<Job>> GetJobsByCustomerIdAsync(int customerId);

        // 👇 NEW: Needed for HR Stats (Total Jobs Count)
        Task<List<Job>> GetAllJobsAsync();
        // 👆 END NEW

        // Applications
        Task<(bool Success, string Message)> ApplyForJobAsync(int jobId, decimal negotiatedRate);
        Task<List<JobApplicationDetail>> GetApplicationsForJobAsync(int jobId);
        Task<(bool Success, string Message)> AcceptApplicationAsync(JobApplicationDetail application);

        // Worker Job Actions
        Task<List<MyJobDetail>> GetMyWorkerJobsAsync();
        Task<(bool Success, string Message)> ClockInAsync(int applicationId);
        Task<(bool Success, string Message, TimeSpan? TotalTime)> ClockOutAsync(int applicationId);

        Task<(bool Success, string Message)> CustomerCompleteJobAsync(int jobId);
        Task<(bool Success, string Message)> CustomerMarkJobIncompleteAsync(int jobId, string reason);

        // Ratings & Reports
        Task RateWorkerOnJobAsync(int applicationId, int rating, string review);
        Task<List<RatingDetail>> GetMyRatingsAsync();
        Task<(bool Success, string Message)> PayWorkerAsync(int applicationId);
        Task<List<Transaction>> GetMyTransactionsAsync();

        // 👇 NEW: Needed for HR Stats (Total Payouts)
        Task<List<Transaction>> GetAllTransactionsAsync();
        // 👆 END NEW

        Task<(bool Success, string Message)> FileWorkerReportAsync(WorkerDetail worker, Job job, string message);
        Task<List<WorkerReport>> GetMyFiledWorkerReportsAsync();
        Task<List<WorkerReport>> GetAllWorkerReportsAsync();
        Task<List<JobWithWorkerDetail>> GetMyJobsWithWorkerAsync();
        Task<List<User>> GetAllCustomersAsync();
    }
}