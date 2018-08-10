using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Database.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.ExceptionsLogs = new HashSet<ExceptionsLog>();
            this.PostedSolutions = new HashSet<PostedSolution>();
            this.UserAttempts = new HashSet<UserAttempt>();
        }

        public Guid? UserInfoId { get; set; }
        public Guid? TeamId { get; set; }
        [MaxLength(15)]
        public string Ip { get; set; }
        [MaxLength(128)]
        public string FirstName { get; set; }
        [MaxLength(128)]
        public string LastName { get; set; }
        [MaxLength(128)]
        public string MiddleName { get; set; }
        public bool IsDeleted { get; set; }
        public int Rating { get; set; }

        public virtual ICollection<ExceptionsLog> ExceptionsLogs { get; set; }
        public virtual ICollection<PostedSolution> PostedSolutions { get; set; }
        public virtual ICollection<UserAttempt> UserAttempts { get; set; }
        public virtual UserInfo UserInfo { get; set; }
        public virtual Team Team { get; set; }
    }
}