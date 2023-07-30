using AngularAuthYtAPI.Context;
using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                FileData = await GetFileData(request.ResumeFile)
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
        public async Task<ActionResult<IEnumerable<JobResumeViewModel>>> GetResumes()
        {
            var jobResumes = await _context.job
                .Where(job => _context.Resumes.Any(resume => resume.JobId == job.JobId))
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
                        .Where(resume => resume.JobId == job.JobId)
                        .Select(resume => new ResumeViewModel
                        {
                            ApplicantName = resume.Name,
                            ApplicantEmail = resume.email,
                            ResumeFileName = resume.FileName,
                            ResumeFileData = Convert.ToBase64String(resume.FileData)
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
        public string ApplicantName { get; set; }
        public string ApplicantEmail { get; set; }
        public string? ResumeFileName { get; set; }
        public string? ResumeFileData { get; set; }
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





}