using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Comp1640.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            CreateAt = DateTime.Now;
        }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public DateTime CreateAt { get; set; } 
        public virtual ICollection<Idea> Ideas { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}