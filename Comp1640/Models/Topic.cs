using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comp1640.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }    
        public DateTime ClosureDate { get; set; }
        public DateTime FinalClosureDate { get; set; }
        public virtual ICollection<Idea> Ideas { get; set; }
    }
}
