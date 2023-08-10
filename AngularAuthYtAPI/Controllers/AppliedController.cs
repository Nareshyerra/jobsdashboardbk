using AngularAuthYtAPI.Context;
using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AngularAuthYtAPI.Controllers.JobsController;

namespace AngularAuthYtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppliedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppliedController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Applied/GetJobsByUser/{username}
        [HttpGet("GetJobsByUser/{AppliedUsername}")]
        public async Task<ActionResult<IEnumerable<applied>>> GetJobsByUser(string AppliedUsername)
        {
            var appliedJobs = await _context.AppliedJob
                .Where(job => job.AppliedUsername == AppliedUsername)
                .ToListAsync();

            return Ok(appliedJobs);
        }






        // POST: api/Applied/ApplyForJob/{username}
        [HttpPost("ApplyForJob/{AppliedUsername}")]
        public async Task<ActionResult<applied>> ApplyForJob(string AppliedUsername, JobsRequest request)
        {
            var jobId = request.JobsObj.JobId;
            var job = await _context.job.FindAsync(jobId);

            if (job == null)
            {
                return NotFound();
            }

            // Check if the user with the provided username exists in your system
            // You can use this information to associate the application with the user

            job.ischecked = true;

            var appliedJob = new applied
            {
                JobId = job.JobId,
                CompanyName = job.CompanyName,
                JobTitle = job.JobTitle,
                Experience = job.Experience,
                Skills = job.Skills,
                JobType = job.JobType,
                PostedDate = DateTime.Now,
                Salary= job.Salary,
                EndDate = DateTime.Now,
                Qualification= job.Qualification,
                Positions= job.Positions,

                Location = job.Location,
                JobDescription = job.JobDescription,

                AppliedUsername = AppliedUsername,
            };

            _context.AppliedJob.Add(appliedJob);
            await _context.SaveChangesAsync();

            return Ok(appliedJob);
        }
    }
}
