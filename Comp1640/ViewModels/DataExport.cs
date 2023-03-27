using Comp1640.Models;

namespace Comp1640.ViewModels
{
    public class DataExport
    {
        public int IdIdea { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; } 
        public string CreateDate { get; set; }
        public string CategoryName { get; set; }
        public string TopicName { get; set; }   
        public string NameUser { get; set; }
        public int numberComment { get;set; }
        public int numberLike { get; set; }
        public int numberDislike { get; set; }
        public int numberView { get; set; }

    }
}
