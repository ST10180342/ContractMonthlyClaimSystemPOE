namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } // e.g., Submitted, Verified, Approved, Settled
        public double HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public List<string> SupportingDocuments { get; set; }
    }
}