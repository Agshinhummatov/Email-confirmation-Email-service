using Elearn_temp.Models;

namespace Elearn_temp.Areas.Admin.ViewModels
{
    public class CourseDetailVM
    {
        public string Name { get; set; }
        
        public string Price { get; set; }

       
        public int CountSale { get; set; }

       
        public string Description { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public ICollection<CourseImage> Images { get; set; }

        public List<IFormFile> Photos { get; set; }
    }
}
