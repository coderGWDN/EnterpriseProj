using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comp1640.Models
{
    public class React
    {
        [Key]
        public int Id { get; set; }
        public bool Dislike { get; set; }
        public bool Like { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public int IdealID { get; set; }
    }
}
