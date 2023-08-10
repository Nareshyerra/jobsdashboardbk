using AngularAuthYtAPI.Models;

namespace AngularAuthYtAPI.Interface
{
    public interface IJobsRepository
    {

        Task<IEnumerable<Jobs>> GetJobsByUser(string username);
        Task<IEnumerable<Jobs>> GetAllJobs(string appliedUsername);

        Task<int> GetTotalJobsCount(string username);
        Task<int> GetTotalAppliedJobsCount(string username);
        Task AddJobsdata(Jobs job);
        Task<Jobs> GetJob(int jobId);
        Task EditJobsdata(Jobs job);
        Task DeleteJobsdata(int jobId);
      
    

    }
}