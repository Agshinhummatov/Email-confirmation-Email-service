using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class BlogVM
    {
        public  IEnumerable<Blog> Blogs { get; set; }
        public  OurBlog OurBlogHeader { get; set; }

    }
}
