using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Comp1640.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Comp1640.ViewModels
{
    public class UpdateUserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [Required] public string Role { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}