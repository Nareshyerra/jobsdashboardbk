using AngularAuthYtAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthYtAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Jobs> job { get; set; }
        //public DbSet<applied> AppliedJobs { get; set; }

        public DbSet<applieds> AppliedJob { get; set; }


        public DbSet<applicant> Applicant { get; set; }

        public DbSet<Resumes> Resumes { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("users");
            builder.Entity<Jobs>().ToTable("job");
            builder.Entity<applicant>().ToTable("Applicant");
            //builder.Entity<applied>().ToTable("AppliedJobs");
            builder.Entity<Resumes>().ToTable("Resumes");

            builder.Entity<applieds>().ToTable("AppliedJob");

        }
    }
}
