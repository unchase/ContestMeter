using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Database.Entities
{
    public class UserInfo
    {
        public UserInfo()
        {
            this.Users = new HashSet<ApplicationUser>();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime LastVisitDate { get; set; }
        [Required]
        [MaxLength(150)]
        public string School { get; set; }
        [Required]
        public int Grade { get; set; }
        [Required]
        [MaxLength(150)]
        public string HomeTown { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
