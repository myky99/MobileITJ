namespace MobileITJ.Models
{
    public class RatingDetail
    {
        public string JobDescription { get; set; } = "";

        // 👇 NEW: Stores the name of the person who rated
        public string CustomerName { get; set; } = "Unknown Client";

        public int Rating { get; set; }
        public string Review { get; set; } = "";
    }
}