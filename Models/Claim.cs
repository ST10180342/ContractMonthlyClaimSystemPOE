
namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public int Id { get; set; }
        public string LecturerName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; } // e.g., Submitted, Verified, Approved, Settled
        public List<string> SupportingDocuments { get; set; } // Placeholder for file paths
    }
}