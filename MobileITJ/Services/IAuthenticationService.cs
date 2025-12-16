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
        Task<(bool Success, string Message)> RegisterAsync(string firstName, string lastName, string email, string password, UserType userType);
        Task<(bool Success, string Message, UserType? UserType, bool IsFirstLogin)> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        // HR & Workers
        Task<(bool Success, string Message, string WorkerId, string TempPassword)> CreateWorkerAsync(string firstName, string lastName, string email, List<string> skills, decimal ratePerHour);
        Task<List<WorkerDetail>> GetAllWorkersAsync();
        Task UpdateWorkerProfileAsync(WorkerDetail worker);
        Task<WorkerDetail?> GetMyWorkerProfileAsync();
        Task<List<User>> GetAllCustomersAsync(); // View Customers

        // Skill Categories
        Task<List<string>> GetSkillCategoriesAsync();
        Task AddSkillCategoryAsync(string category);
        Task RemoveSkillCategoryAsync(string category);

        // Jobs
        Task<(bool Success, string Message)> CreateJobAsync(Job newJob);
        Task<List<CustomerJobDetail>> GetMyCustomerJobsAsync();
        Task<List<Job>> GetAvailableJobsAsync();
        Task<List<Job>> GetMyJobsAsync();
        Task<List<Job>> GetJobsByCustomerIdAsync(int customerId);
        Task<List<Job>> GetAllJobsAsync(); // For Stats

        // Applications
        Task<(bool Success, string Message)> ApplyForJobAsync(int jobId, decimal negotiatedRate);
        Task<List<JobApplicationDetail>> GetApplicationsForJobAsync(int jobId);
        Task<(bool Success, string Message)> AcceptApplicationAsync(JobApplicationDetail application);

        // 👇 THIS WAS MISSING
        Task<(bool Success, string Message)> RejectApplicationAsync(int applicationId);
        // 👆 END FIX

        // Job Execution (Worker)
        Task<List<MyJobDetail>> GetMyWorkerJobsAsync();
        Task<(bool Success, string Message)> ClockInAsync(int applicationId);
        Task<(bool Success, string Message, TimeSpan? TotalTime)> ClockOutAsync(int applicationId);
        Task<List<JobApplicationDetail>> GetWorkerJobHistoryAsync(int workerUserId);

        // Job Completion & Reporting (Customer)
        Task<(bool Success, string Message)> CustomerCompleteJobAsync(int jobId);
        Task<(bool Success, string Message)> CustomerMarkJobIncompleteAsync(int jobId, string reason);
        Task RateWorkerOnJobAsync(int applicationId, int rating, string review);
        Task<List<RatingDetail>> GetMyRatingsAsync(); // Worker Ratings view

        // Payments & Reports
        Task<(bool Success, string Message)> PayWorkerAsync(int applicationId);
        Task<List<Transaction>> GetMyTransactionsAsync();
        Task<List<Transaction>> GetAllTransactionsAsync(); // For Stats
        Task<(bool Success, string Message)> FileWorkerReportAsync(WorkerDetail worker, Job job, string message);
        Task<List<WorkerReport>> GetMyFiledWorkerReportsAsync();
        Task<List<WorkerReport>> GetAllWorkerReportsAsync();
        Task<List<JobWithWorkerDetail>> GetMyJobsWithWorkerAsync();
    }
}