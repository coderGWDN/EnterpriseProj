using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Comp1640.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Idea> Ideas { get; set; }
    }
}
