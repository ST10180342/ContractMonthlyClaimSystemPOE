using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystemPOE.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required]
        public string LecturerId { get; set; } = string.Empty;

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Hours must be greater than 0.")]
        public double HoursWorked { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0.")]
        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; }

        public string? FilePath { get; set; }
        public string? FileName { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public virtual ApplicationUser? Lecturer { get; set; }
    }
}