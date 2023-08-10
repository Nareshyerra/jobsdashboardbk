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
     

        public DbSet<applied> AppliedJob { get; set; }


        public DbSet<applicant> Applicant { get; set; }

        public DbSet<Resumes> Resumes { get; set; }

        public DbSet<StatusClass> StatusClass { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("users");
            builder.Entity<Jobs>().ToTable("job");
            builder.Entity<applicant>().ToTable("Applicant");
           
            builder.Entity<Resumes>().ToTable("Resumes");

            builder.Entity<applied>().ToTable("AppliedJob");

            builder.Entity<StatusClass>().ToTable("StatusClass");



        }
    }
}
