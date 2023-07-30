using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAuthYtAPI.Models
{
    public class applieds
    {
        [Key]
        public int AppliedId { get; set; } // Primary key for the Applied table

        public int JobId { get; set; } // Foreign key to the Jobs table

        [ForeignKey("JobId")]
        public Jobs Job { get; set; }

        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? Experience { get; set; }
        public string? Skills { get; set; }
        public string? JobType { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? Location { get; set; }
        public string? JobDescription { get; set; }

        public string? AppliedUsername { get; set; }
    }
}
