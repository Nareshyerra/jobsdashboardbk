using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AngularAuthYtAPI.Migrations;

namespace AngularAuthYtAPI.Models
{
    public class Resumes
    {
        [Key]
        public int ResumeId { get; set; }

        public string Name { get; set; }

        public string email { get; set; }

        public int JobId { get; set; } // Foreign key to the Applied table

        [ForeignKey("JobId")]
        public Jobs job { get; set; }
        public string? FileName { get; set; } // Name of the uploaded resume file

        public string? FilePath { get; set; }
        public byte[] FileData { get; set; } // Binary data of the uploaded resume file



    }
}