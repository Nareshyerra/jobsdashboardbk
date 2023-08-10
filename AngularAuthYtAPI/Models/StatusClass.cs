using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAuthYtAPI.Models
{
    public class StatusClass
    {
        [Key]
        public int statusId { get; set; }

        public int JobId { get; set; }

        public int ResumeId { get; set; }
        public string Status { get; set; }
    }
}
