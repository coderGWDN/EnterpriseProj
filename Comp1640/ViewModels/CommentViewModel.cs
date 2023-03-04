using System.ComponentModel.DataAnnotations;
using System;

namespace Comp1640.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int IdealID { get; set; }
    }
}
