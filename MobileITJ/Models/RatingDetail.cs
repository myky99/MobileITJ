namespace MobileITJ.Models
{
    public class RatingDetail
    {
        public string JobDescription { get; set; } = ""; // Stores Job Title
        public int Rating { get; set; }
        public string Review { get; set; } = "";

        // 👇 NEW: Store the Customer's Name
        public string CustomerName { get; set; } = "Unknown Client";
    }
}