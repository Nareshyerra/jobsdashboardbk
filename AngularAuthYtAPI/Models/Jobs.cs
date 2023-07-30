using System.ComponentModel.DataAnnotations;

namespace AngularAuthYtAPI.Models
{
    public class Jobs
    {
        [Key]
        public int JobId { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? Experience { get; set; }

        public string? Skills { get; set; }

        public string? JobType { get; set; }
        public DateTime? PostedDate { get; set; }


        public string? Location { get; set; }

        public string? JobDescription { get; set; }

        public bool ischecked { get; set; }

        public string? Username { get; set; }

    }
}
