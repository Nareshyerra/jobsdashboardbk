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
using AngularAuthYtAPI.Interface;

namespace AngularAuthYtAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsRepository _jobsRepository;

        public JobsController(IJobsRepository jobsRepository)
        {
            _jobsRepository = jobsRepository;
        }



        [HttpGet("Jobs")]
        public async Task<ActionResult<IEnumerable<Jobs>>> GetAllJobs(string appliedUsername)
        {
            var jobs = await _jobsRepository.GetAllJobs(appliedUsername);

            return Ok(jobs);
        }





        [HttpGet("count/{username}")]
        public async Task<IActionResult> GetTotalJobsCount(string username)
        {
            int jobsCount = await _jobsRepository.GetTotalJobsCount(username);
            return Ok(jobsCount);
        }



        [HttpGet("TotalAppliedJobsCount/{username}")]
        public async Task<int> GetTotalAppliedJobsCount(string username)
        {
            return await _jobsRepository.GetTotalAppliedJobsCount(username);
        }

    


        [HttpGet("NotAppliedJobsCount/{username}")]
        public async Task<int> GetNotAppliedJobsCount(string username)
        {
            var totalJobsCount = await _jobsRepository.GetTotalJobsCount(username);
            var appliedJobsCount = await _jobsRepository.GetTotalAppliedJobsCount(username);
            var notAppliedJobsCount = totalJobsCount - appliedJobsCount;
            return notAppliedJobsCount;
        }




        [HttpGet]
        public async Task<ActionResult<IEnumerable<Jobs>>> GetJobsByUser(string username)
        {
            var jobs = await _jobsRepository.GetJobsByUser(username);
            return Ok(jobs);
        }




        [HttpPost]
        public async Task<IActionResult> AddJobsdata(Jobs job)
        {
            job.PostedDate = DateTime.UtcNow;
            job.EndDate = DateTime.UtcNow;
            await _jobsRepository.AddJobsdata(job);
            return Ok();
        }


        [HttpPut("editjobsdata/{jobId}")]
        public async Task<IActionResult> EditJobsdata(int jobId, Jobs updatedJob)
        {
            // Retrieve the existing job from the database
            var existingJob = await _jobsRepository.GetJob(jobId);
            if (existingJob == null)
                return NotFound();

            // Update only the specified properties from the updatedJob object
            existingJob.CompanyName = updatedJob.CompanyName;
            existingJob.JobTitle = updatedJob.JobTitle;
            existingJob.Experience = updatedJob.Experience;
            existingJob.Skills = updatedJob.Skills;
            existingJob.JobType = updatedJob.JobType;
            existingJob.Qualification = updatedJob.Qualification;
            existingJob.Location = updatedJob.Location;
            existingJob.Positions = updatedJob.Positions;
            existingJob.EndDate = updatedJob.EndDate;
            existingJob.PostedDate = updatedJob.PostedDate;

            existingJob.Salary = updatedJob.Salary;
            existingJob.JobDescription = updatedJob.JobDescription;

            await _jobsRepository.EditJobsdata(existingJob);
            return Ok();
        }



        [HttpDelete("deletejobsdata/{jobId}")]
        public async Task<IActionResult> DeleteJobsdata(int jobId)
        {
            await _jobsRepository.DeleteJobsdata(jobId);
            return Ok();
        }

        public class JobsRequest
        {
            public Jobs JobsObj { get; set; }

        }





    }
}