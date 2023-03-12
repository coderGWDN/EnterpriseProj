
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comp1640.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public int IdealID { get; set; }
     
    }
}
