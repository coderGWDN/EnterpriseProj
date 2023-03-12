
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comp1640.Models
{
    public class View
    {
        [Key]
        public int Id { get; set; }
        public DateTime VisitDate { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public int IdealID { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
