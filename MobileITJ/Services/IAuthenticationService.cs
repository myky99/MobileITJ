using System.Threading.Tasks;
using MobileITJ.Models;
using System.Collections.Generic;
using System;

namespace MobileITJ.Services
{
    public interface IAuthenticationService
    {
        Task InitializeAsync();
        Task<(bool Success, string Message)> RegisterAsync(string firstName, string lastName, string email, string password, UserType userType);
        Task<(bool Success, string Message, string WorkerId, string TempPassword)> CreateWorkerAsync(string firstName, string lastName, string email, List<string> skills, decimal ratePerHour);
        Task<List<WorkerDetail>> GetAllWorkersAsync();
        Task UpdateWorkerProfileAsync(WorkerDetail worker);
        Task<List<User>> GetAllCustomersAsync();
        Task<(bool Success, string Message)> CreateJobAsync(Job newJob);
        Task<List<Job>> GetMyJobsAsync();
        Task<List<CustomerJobDetail>> GetMyCustomerJobsAsync();
        Task<(bool Success, string Message)> ClockInAsync(int applicationId);
        Task<(bool Success, string Message, TimeSpan? TotalTime)> ClockOutAsync(int applicationId);
        Task RateWorkerOnJobAsync(int applicationId, int rating, string review);
        Task<(bool Success, string Message)> PayWorkerAsync(int applicationId);
        Task<List<Job>> GetAvailableJobsAsync();
        Task<(bool Success, string Message)> FileWorkerReportAsync(WorkerDetail worker, string message);
        Task<List<WorkerReport>> GetMyFiledWorkerReportsAsync();
        Task<(bool Success, string Message)> ApplyForJobAsync(int jobId, decimal negotiatedRate);
        Task<List<MyJobDetail>> GetMyWorkerJobsAsync();
        Task<WorkerDetail?> GetMyWorkerProfileAsync();
        Task<List<RatingDetail>> GetMyRatingsAsync();
        Task<List<JobApplicationDetail>> GetApplicationsForJobAsync(int jobId);
        Task<(bool Success, string Message)> AcceptApplicationAsync(JobApplicationDetail application);
        Task<List<JobWithWorkerDetail>> GetMyJobsWithWorkerAsync();
        Task<List<Transaction>> GetMyTransactionsAsync();
        Task<(bool Success, string Message)> CustomerCompleteJobAsync(int jobId);

        // --- 👇 ADD THIS NEW SIGNATURE 👇 ---
        Task<List<WorkerReport>> GetAllWorkerReportsAsync();
        // --- END OF NEW ---

        Task<(bool Success, string Message, UserType? UserType, bool IsFirstLogin)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsAuthenticatedAsync();
        Task LogoutAsync();
    }
}