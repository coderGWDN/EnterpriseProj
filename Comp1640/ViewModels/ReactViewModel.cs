using System.ComponentModel.DataAnnotations;

namespace Comp1640.ViewModels
{
    public class ReactViewModel
    {
        [Required]
        public int Like { get; set; }
        [Required]
        public int DisLike { get; set; }
        [Required]
        public int IdeaId { get; set; }
    }
}
