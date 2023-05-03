using Elearn_temp.Models;

namespace Elearn_temp.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }

        public IEnumerable<Author> Authors { get; set; }

        public IEnumerable<Course> Courses { get; set; }

        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<News> News { get; set; }
    }
}
