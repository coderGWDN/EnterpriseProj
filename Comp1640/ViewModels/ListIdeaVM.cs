using Comp1640.Models;
using System.Collections.Generic;

namespace Comp1640.ViewModels
{
    public class ListIdeaVM
    {
        public Idea Idea { get; set; }
        public CommentViewModel Comment { get; set; }

        public List<Comment> ListComment { get; set; }

        public React React { get; set; }

        public List<React> ListReactLike { get; set; }
        public List<React> ListReactDislike { get; set; }

        public View View { get; set; }

        public List<View> ListView { get; set; }

    }
}
