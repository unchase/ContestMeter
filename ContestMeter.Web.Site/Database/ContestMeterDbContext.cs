using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ContestMeter.Web.Site.Database.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Database
{
    public class ContestMeterDbContext : IdentityDbContext<ApplicationUser>
    {
        public ContestMeterDbContext()
            : base("name=ContestMeterConnection")
        {
            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            //DbContext.Configuration.ProxyCreationEnabled = false;
            ////DbContext.Configuration.LazyLoadingEnabled = true; // не имеет значение, если ProxyCreationEnabled == false
            //DbContext.Configuration.UseDatabaseNullSemantics = false; //(((operand1 = operand2) AND (NOT (operand1 IS NULL OR operand2 IS NULL))) OR ((operand1 IS NULL) AND (operand2 IS NULL)))
            //DbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestsType> ContestsTypes { get; set; }
        public DbSet<DevelopmentTool> DevelopmentTools { get; set; }
        public DbSet<ExceptionsLog> ExceptionsLogs { get; set; }
        public DbSet<PostedSolution> PostedSolutions { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserAttempt> UserAttempts { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<ContestTeacher> ContestTeachers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamContest> TeamContests { get; set; }
        public DbSet<TeamParticipant> TeamParticipants { get; set; } 
        //public DbSet<ApplicationUser> Users { get; set; }
    }
}