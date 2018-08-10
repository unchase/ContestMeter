using System.Collections.Generic;
using ContestMeter.Web.Site.Database.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ContestMeter.Web.Site.Database.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ContestMeter.Web.Site.Database.ContestMeterDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = @"Database\Migrations";
        }

        protected override void Seed(ContestMeter.Web.Site.Database.ContestMeterDbContext context)
        {
            // ��������� � ���� ������ ���� "administrator", "teacher" � "participant"
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            if (!context.Roles.Any(r => r.Name == "administrator"))
            {
                roleManager.Create(new IdentityRole { Name = "administrator" });
            }
            if (!context.Roles.Any(r => r.Name == "teacher"))
            {
                roleManager.Create(new IdentityRole { Name = "teacher" });
            }
            if (!context.Roles.Any(r => r.Name == "participant"))
            {
                roleManager.Create(new IdentityRole { Name = "participant" });
            }

            // ��������� ������������ "admin" � ������� "adminadmin" � ���� ������, ���� �� �� ����������,
            // � ���������� ��� ���� "administrator"
            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = new ApplicationUser { UserName = "admin" };

                userManager.Create(user, "adminadmin");
                userManager.AddToRole(user.Id, "administrator");
            }

            // ��������� � ���� ������ ��������� ���� ���������: "KYROV" � "ACM", ���� ��� �� ����������
            if (!context.ContestsTypes.Any(ct => ct.Name == "KYROV"))
            {
                context.ContestsTypes.Add(new ContestsType { Id = Guid.NewGuid(), Name = "KYROV" });
            }
            if (!context.ContestsTypes.Any(ct => ct.Name == "ACM"))
            {
                context.ContestsTypes.Add(new ContestsType { Id = Guid.NewGuid(), Name = "ACM" });
            }

            // ��������� � ���� ������ ������� ��-���������, � ������� ����� �������������� ��������� ������������, ���� ��� �� ����������
            if (!context.Teams.Any(ct => ct.Name == "��������"))
            {
                context.Teams.Add(new Team { Id = Guid.NewGuid(), Name = "��������", MaxTeamNumber = 10000, Rating = 0, Contests = new List<Contest>(), Participants = new List<ApplicationUser>()});
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
