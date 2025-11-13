namespace MobileITJ.Models
{
    public enum UserType
    {
        Customer,
        Worker,
        HR
    }

    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }   // ✅ Added for authentication
        public UserType UserType { get; set; }
    }
}
