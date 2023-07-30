using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularAuthYtAPI.Models;
using AngularAuthYtAPI.Context;
using System.Security.Claims;

namespace AngularAuthYtAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContext _context;
        


        public JobsController(AppDbContext context)
        {
            _context = context;
          
        }


        //[HttpGet("Jobs")]
        //public async Task<ActionResult<IEnumerable<Jobs>>> GetAllJobs()
        //{
        //    // Retrieve all jobs and return as ActionResult
        //    var jobs = await _context.job.ToListAsync();
        //    return Ok(jobs);
        //}

        [HttpGet("Jobs")]
        public async Task<ActionResult<IEnumerable<Jobs>>> GetAllJobs(string AppliedUsername)
        {
            var jobs = await _context.job.ToListAsync();

            if (!string.IsNullOrEmpty(AppliedUsername))
            {
                var AppliedId = await _context.AppliedJob
                    .Where(applieds => applieds.AppliedUsername == AppliedUsername)
                    .Select(applieds => applieds.JobId)
                    .ToListAsync();

                // Update the ischecked property based on appliedJobIds
                foreach (var job in jobs)
                {
                    job.ischecked = AppliedId.Contains(job.JobId);
                }
            }
            else
            {
                // If no username provided, set ischecked to false for all jobs
                foreach (var job in jobs)
                {
                    job.ischecked = false;
                }
            }

            return Ok(jobs);
        }








        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jobs>>> GetJobsByUser(string username)
        {
            var jobs = await _context.job.Where(job => job.Username == username).ToListAsync();
            return Ok(jobs);
        }





        [HttpPost("Addjobsdata")]

        public async Task<IActionResult> AddJobsdata([FromBody] Jobs jobsObj)

        {

            if (jobsObj == null)

                return BadRequest();



            await _context.job.AddAsync(jobsObj);

            await _context.SaveChangesAsync();



            return Ok(new

            {

                Message = "Job Added Successfully!"

            });

        }




        [HttpPut("editjobsdata/{id}")]
        public async Task<IActionResult> EditJobsdata(int id, [FromBody] Models.Jobs updatedJobsObj)
        {
            if (updatedJobsObj == null || id != updatedJobsObj.JobId)
                return BadRequest();



            var job = await _context.job.FindAsync(id);
            if (job == null)
                return NotFound();



            job.CompanyName = updatedJobsObj.CompanyName;
            job.JobTitle = updatedJobsObj.JobTitle;
            job.Experience = updatedJobsObj.Experience;
            job.Skills = updatedJobsObj.Skills;
            job.JobType = updatedJobsObj.JobType;
            job.PostedDate = updatedJobsObj.PostedDate;
            job.Location = updatedJobsObj.Location;
            job.JobDescription = updatedJobsObj.JobDescription;






            await _context.SaveChangesAsync();



            return Ok(new
            {
                Message = "Job Updated Successfully!"
            });
        }




        [HttpDelete("deletejobsdata/{id}")]
        public async Task<IActionResult> DeleteJobsdata(int id)
        {
            var job = await _context.job.FindAsync(id);
            if (job == null)
                return NotFound();



            _context.job.Remove(job);
            await _context.SaveChangesAsync();



            return Ok(new
            {
                Message = "Job Deleted Successfully!"
            });
        }


        public class JobsRequest
        {
            public Jobs JobsObj { get; set; }

        }





    }
}