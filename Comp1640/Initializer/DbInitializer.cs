using System;
using System.Linq;
using Comp1640.Data;
using Comp1640.Models;
using Comp1640.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Comp1640.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initializer()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            
            if (_db.Roles.Any(r => r.Name == SD.Role_QA_COORDINATOR)) return;
            if (_db.Roles.Any(r => r.Name == SD.Role_QA_MANAGER)) return;
            if (_db.Roles.Any(r => r.Name == SD.Role_STAFF)) return;
            if (_db.Roles.Any(r => r.Name == SD.Role_ADMIN)) return;
            
            _roleManager.CreateAsync(new IdentityRole(SD.Role_QA_COORDINATOR)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_QA_MANAGER)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_STAFF)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_ADMIN)).GetAwaiter().GetResult();

            // create default department
            var department = new Department()
            {
                Name = "Default"
            };

            _db.Add(department);
            _db.SaveChanges();

            _userManager.CreateAsync(new ApplicationUser()
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true, 
                FullName = "Admin",
                Address = "Đà Nẵng",
                DepartmentId = department.Id
            }, "Admin123@").GetAwaiter().GetResult();
            
            ApplicationUser admin = _db.ApplicationUsers.Where(u => u.Email == "admin@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(admin, SD.Role_ADMIN).GetAwaiter().GetResult();
        }
    }
}