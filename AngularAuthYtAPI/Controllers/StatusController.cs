using AngularAuthYtAPI.Context;
using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AngularAuthYtAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{resumeId}/{jobId}")]
        public async Task<IActionResult> AddStatus(int resumeId, int jobId, [FromBody] StatusClass status)
        {
            try
            {
                var resume = await _context.Resumes.FindAsync(resumeId);

                if (resume == null)
                {
                    return NotFound("Resume not found.");
                }

                // You can add validation checks for the status data if needed
                if (status == null || string.IsNullOrWhiteSpace(status.Status))
                {
                    return BadRequest("Invalid status data.");
                }

                // Save the status in the database
                status.ResumeId = resumeId;
                status.JobId = jobId; // Assign jobId to the StatusClass
                _context.StatusClass.Add(status);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Status added successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "An error occurred while adding status.");
            }
        }

        [HttpGet("{resumeId}/{jobId}")]
        public async Task<IActionResult> GetStatus(int resumeId, int jobId)
        {
            var status = await _context.StatusClass.FirstOrDefaultAsync(s => s.ResumeId == resumeId && s.JobId == jobId);

            if (status == null)
            {
                return NotFound("Status not found for the given resumeId and jobId.");
            }

            return Ok(status);
        }
    }
}
