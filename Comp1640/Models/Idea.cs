using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comp1640.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; } 
        public string Content { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }
        [Required]
        public int TopicID { get; set; }
        [Required]
        [ForeignKey("TopicID")]  
        
        public Topic Topic { get; set; }
        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; }

    }
}
