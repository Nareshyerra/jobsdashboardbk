using AngularAuthYtAPI.Context;
using AngularAuthYtAPI.Interface;
using AngularAuthYtAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthYtAPI.Respository
{
    public class JobsRepository : IJobsRepository
    {
        private readonly AppDbContext _dbContext;

        public JobsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Jobs>> GetAllJobs()
        {
            return await _dbContext.job.ToListAsync();
        }





        

        public async Task<int> GetTotalJobsCount(string username)
        {
            return await _dbContext.job.CountAsync(j => j.Username == username);
        }


       

        public async Task<int> GetTotalAppliedJobsCount(string username)
        {
            var appliedJobsCount = await _dbContext.job
                .Where(job => _dbContext.Resumes.Any(resume => resume.JobId == job.JobId && job.Username == username))
                .CountAsync();

            return appliedJobsCount;
        }


    






        public async Task AddJobsdata(Jobs job)
        {
            _dbContext.job.Add(job);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Jobs> GetJob(int jobId)
        {
            return await _dbContext.job.FindAsync(jobId);
        }

        public async Task EditJobsdata(Jobs job)
        {
            _dbContext.Entry(job).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteJobsdata(int jobId)
        {
            var job = await _dbContext.job.FindAsync(jobId);
            if (job != null)
            {
                _dbContext.job.Remove(job);
                await _dbContext.SaveChangesAsync();
            }
        }

      
               public async Task<IEnumerable<Jobs>> GetAllJobs(string appliedUsername)
        {
            var jobs = await _dbContext.job.ToListAsync();

            if (!string.IsNullOrEmpty(appliedUsername))
            {
                var appliedIds = await _dbContext.AppliedJob
                    .Where(applied => applied.AppliedUsername == appliedUsername)
                    .Select(applied => applied.JobId)
                    .ToListAsync();

                // Update the ischecked property based on appliedJobIds
                foreach (var job in jobs)
                {
                    job.ischecked = appliedIds.Contains(job.JobId);
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

            return jobs;
        }


        public async Task<IEnumerable<Jobs>> GetJobsByUser(string username)
        {
            return await _dbContext.job.Where(job => job.Username == username).ToListAsync();
        }

    }
}