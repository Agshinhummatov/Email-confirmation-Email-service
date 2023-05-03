using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class FooterVM
    {
        public IEnumerable<Social> Socials { get; set; } 

        public Dictionary<string, string> Settings { get; set; }
    }
}
