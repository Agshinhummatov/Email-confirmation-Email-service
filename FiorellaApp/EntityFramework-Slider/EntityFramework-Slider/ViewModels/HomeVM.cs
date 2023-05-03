using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public About Abouts { get; set; }
        public IEnumerable<Adventage> Adventages { get; set; }
        public Subscribe Subscribe { get; set; }
        //public OurBlog OurBlog { get; set; }
        public IEnumerable<Say> Says { get; set; }
        public IEnumerable<Instagram> Instagrams { get; set; }


    }
}
