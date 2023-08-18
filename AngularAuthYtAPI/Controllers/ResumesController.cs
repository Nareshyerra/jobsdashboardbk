using AngularAuthYtAPI.Context;
using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace AngularAuthYtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResumesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("{jobId}")]
        public async Task<IActionResult> PostResume(int jobId, [FromForm] ResumeRequest request)
        {
            if (request.ResumeFile == null || request.ResumeFile.Length == 0)
            {
                return BadRequest("Resume file is required.");
            }

            var appliedJob = await _context.job.FindAsync(jobId);

            if (appliedJob == null)
            {
                return NotFound("Applied job not found.");
            }

            var resume = new Resumes
            {
                Name = request.Name,
                email = request.Email,
                JobId = jobId,
                FileName = request.ResumeFile.FileName,
                FileData = await GetFileData(request.ResumeFile),
                AppliedDate = DateTime.UtcNow
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResume", new { id = resume.ResumeId }, resume);
        }

        private async Task<byte[]> GetFileData(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobResumeViewModel>>> GetResumes(string username)
        {
            var jobResumes = await _context.job
                .Where(job => _context.Resumes.Any(resume => resume.JobId == job.JobId && job.Username == username))
                .Select(job => new JobResumeViewModel
                {
                    JobId = job.JobId,
                    CompanyName = job.CompanyName,
                    JobTitle = job.JobTitle,
                    Experience = job.Experience,
                    Skills = job.Skills,
                    JobType = job.JobType,
                    PostedDate = job.PostedDate,
                    Location = job.Location,
                    JobDescription = job.JobDescription,
                    Resumes = _context.Resumes
                        .Where(resume => resume.JobId == job.JobId && job.Username == username) // Filter by username
                        .Select(resume => new ResumeViewModel
                        {
                            ResumeId = resume.ResumeId,
                            ApplicantName = resume.Name,
                            ApplicantEmail = resume.email,
                            ResumeFileName = resume.FileName,
                            AppliedDate = resume.AppliedDate,
                            ResumeFileData = Convert.ToBase64String(resume.FileData),

                           ScheduleMeetingDate = resume.ScheduleMeetingDate,

                            RejectionReason = resume.RejectionReason,
                            Status= resume.Status,
                            IsStatusSelected=resume.IsStatusSelected,

                        })
                        .ToList()
                })
                .ToListAsync();

            return jobResumes;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Resumes>> GetResume(int id)
        {
            var resume = await _context.Resumes.FindAsync(id);

            if (resume == null)
            {
                return NotFound();
            }

            return resume;
        }





        [HttpGet("{jobId}/Download")]
        public async Task<IActionResult> DownloadResume(int jobId)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.JobId == jobId);

            if (resume == null)
            {
                return NotFound();
            }

            var memoryStream = new MemoryStream(resume.FileData);
            return File(memoryStream, "application/octet-stream", resume.FileName);
        }

        
        
        //status

        [HttpPost("{resumeId}/ScheduleMeeting")]
        public async Task<IActionResult> ScheduleMeeting(int resumeId, [FromBody] ScheduleMeetingRequest request)
        {
            var resume = await _context.Resumes.FindAsync(resumeId);
            if (resume == null)
            {
                return NotFound();
            }
        
            resume.ScheduleMeetingDate = DateTime.UtcNow;
            resume.Status = "Meeting Scheduled";
            resume.IsStatusSelected = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Meeting has been scheduled successfully." });
        }

        [HttpPost("{resumeId}/Reject")]
        public async Task<IActionResult> RejectResume(int resumeId, [FromBody] RejectResumeRequest request)
        {
            var resume = await _context.Resumes.FindAsync(resumeId);

            if (resume == null)
            {
                return NotFound();
            }

            resume.RejectionReason = request.RejectionReason;
            resume.Status = "Rejected"; 
            resume.IsStatusSelected = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Resume has been Rejected successfully." });
        }



        [HttpGet("{resumeId}/ScheduleMeeting")]
        public async Task<ActionResult<DateTime?>> GetScheduleMeetingDate(int resumeId)
        {
            var resume = await _context.Resumes.FindAsync(resumeId);

            if (resume == null)
            {
                return NotFound();
            }

            if (resume.ScheduleMeetingDate == null)
            {
                return Ok("Meeting date not yet selected.");
            }

            return Ok(resume.ScheduleMeetingDate);
        }

        [HttpGet("{resumeId}/Reject")]
        public async Task<ActionResult<string>> GetRejectionReason(int resumeId)
        {
            var resume = await _context.Resumes.FindAsync(resumeId);

            if (resume == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(resume.RejectionReason))
            {
                return Ok("Rejection reason not yet posted.");
            }

            return Ok(resume.RejectionReason);
        }


        [HttpGet("GetScheduleMeetingDateByJobAndName")]
        public async Task<ActionResult<DateTime?>> GetScheduleMeetingDateByJobAndName(int jobId, string name)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.JobId == jobId && r.Name == name);

            if (resume == null)
            {
                return NotFound();
            }

            if (resume.ScheduleMeetingDate == null)
            {
                return Ok("Meeting date not yet selected.");
            }

            return Ok(resume.ScheduleMeetingDate);
        }




        [HttpGet("GetRejectionReasonByJobAndName")]
        public async Task<ActionResult<string>> GetRejectionReasonByJobAndName(int jobId, string name)
        {
            var resume = await _context.Resumes.FirstOrDefaultAsync(r => r.JobId == jobId && r.Name == name);

            if (resume == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(resume.RejectionReason))
            {
                //return Ok("Rejection reason not yet posted.");
                return Ok(new { RejectionReason = "Rejection reason not yet posted." });

            }

          
            return Ok(new { RejectionReason = resume.RejectionReason });

        }

        [HttpGet("RecentApplicants")]
        public async Task<ActionResult<IEnumerable<RecentApplicantViewModel>>> GetRecentApplicants()
        {
           
            DateTime startDate = DateTime.UtcNow.AddDays(-30);

            var recentApplicants = await _context.Resumes
                .Where(resume =>
                    resume.Name != null &&
                    resume.email != null &&
                    resume.job.JobTitle != null &&
                    resume.job.Location != null)

                .Select(resume => new RecentApplicantViewModel
                {
                    ApplicantName = resume.Name,
                    ApplicantEmail = resume.email,
                    JobTitle = resume.job.JobTitle,
                    CompanyLocation = resume.job.Location,
                    Skills = resume.job.Skills,
                    AppliedDate = (DateTime)resume.AppliedDate // Add this line to include the applicationDate property
                })
                .OrderByDescending(applicant => applicant.AppliedDate) // Sort by ApplicationDate in descending order
                .Take(10) // Take the first 10 records
                .ToListAsync();

            return recentApplicants;
        }

      



        [HttpGet("TotalApplicantsCount/{username}")]
        public async Task<int> GetTotalApplicantsCount(string username)
        {
            int applicantsCount = await _context.job
                .Where(job => job.Username == username)
                .Join(
                    _context.Resumes,
                    job => job.JobId,
                    resume => resume.JobId,
                    (_, resumes) => resumes
                )
                .CountAsync();

            return applicantsCount;
        }





        [HttpGet("AppliedJobsCount")]
        public async Task<int> GetTotalAppliedJobsCount(string username)
        {
            return await _context.job
                .Where(job => _context.Resumes.Any(resume => resume.JobId == job.JobId && job.Username == username))
                .CountAsync();
        }


        //charts
        [HttpGet("ApplicantsPerWeek")]
        public async Task<ActionResult<List<WeekApplicantCount>>> GetApplicantsPerWeek(string username, int year, int week)
        {
            var applicantsWithAppliedDate = await _context.Resumes
                .Where(resume =>
                    resume.job.Username == username &&
                    resume.AppliedDate.HasValue &&
                    resume.AppliedDate.Value.Year == year)
                .ToListAsync();



            var weekCounts = new List<WeekApplicantCount>();



            // Calculate the current ISO week number for reference
            var currentWeek = GetIsoWeekOfYear(DateTime.UtcNow);



            // Calculate the starting week for the range (selected week - 4)
            var startingWeek = week - 4;



            // Adjust the starting week if it's before week 1
            if (startingWeek < 1)
            {
                startingWeek = 1;
            }



            // Calculate the ending week for the range (selected week)
            var endingWeek = week;



            // Iterate through the specified range of weeks
            for (int weekNumber = startingWeek; weekNumber <= endingWeek; weekNumber++)
            {
                var applicantCount = applicantsWithAppliedDate
                    .Count(resume => GetIsoWeekOfYear(resume.AppliedDate.Value) == weekNumber);



                weekCounts.Add(new WeekApplicantCount
                {
                    WeekNumber = weekNumber,
                    ApplicantCount = applicantCount
                });
            }



            return Ok(weekCounts);
        }


        [HttpGet("SchedulingMeetingCount")]
        public async Task<ActionResult<int>> GetSchedulingMeetingCount(string username)
        {
            int schedulingMeetingCount = await _context.Resumes
                .Where(resume => resume.job.Username == username && resume.ScheduleMeetingDate.HasValue)
                .CountAsync();



            return schedulingMeetingCount;
        }



        [HttpGet("NonScheduledMeetingCount")]
        public async Task<ActionResult<int>> GetNonScheduledMeetingCount(string username)
        {
            int nonScheduledMeetingCount = await _context.Resumes
                .Where(resume => resume.job.Username == username && !resume.ScheduleMeetingDate.HasValue)
                .CountAsync();



            return nonScheduledMeetingCount;
        }



        [HttpGet("RejectedCount")]
        public async Task<ActionResult<int>> GetRejectedCount(string username)
        {
            int rejectedCount = await _context.Resumes
                .Where(resume => resume.job.Username == username && resume.Status == "Rejected")
                .CountAsync();



            return rejectedCount;
        }
        [HttpGet("NoStatusCount")]
        public async Task<ActionResult<int>> GetNoStatusCount(string username)
        {
            int noStatusCount = await _context.Resumes
                .Where(resume => resume.job.Username == username && !resume.IsStatusSelected)
                .CountAsync();



            return noStatusCount;
        }


        private int GetIsoWeekOfYear(DateTime date)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var day = (int)cal.GetDayOfWeek(date);
            date = date.AddDays(4 - day).Date;
            return cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public class WeekApplicantCount
        {
            public int WeekNumber { get; set; }
            public int ApplicantCount { get; set; }
        }


    }



    public class RecentApplicantViewModel
    {
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string JobTitle { get; set; }
        public string CompanyLocation { get; set; }
        public string Skills { get; set; }
        public DateTime AppliedDate { get; set; }
    }


    public class ResumeRequest
    {
        public string Name { get; set; }

        //public int JobId { get; set; }
        public string Email { get; set; }
        public IFormFile ResumeFile { get; set; }
    }
    public class ResumeViewModel

    {
        public int ResumeId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string? ResumeFileName { get; set; }
        public string? ResumeFileData { get; set; }

        public DateTime? AppliedDate { get; set; }

        public DateTime? ScheduleMeetingDate { get; set; }
        public string? RejectionReason { get; set; }

        public string? Status { get; set; }

        public int JobId { get; set; }

        public bool IsStatusSelected { get; set; }

      


    }

    public class JobResumeViewModel
    {
        public int JobId { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? Experience { get; set; }
        public string? Skills { get; set; }
        public string? JobType { get; set; }
        public DateTime? PostedDate { get; set; }
        public string? Location { get; set; }
        public string? JobDescription { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string? ResumeFileName { get; set; }
        public List<ResumeViewModel> Resumes { get; set; }
    }


    public class ScheduleMeetingRequest
    {
        public DateTime ScheduleMeetingDate { get; set; }
    }

    public class RejectResumeRequest
    {
        public string RejectionReason { get; set; }
    }


}