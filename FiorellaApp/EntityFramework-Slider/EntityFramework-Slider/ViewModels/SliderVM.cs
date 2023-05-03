using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.ViewModels
{
    public class SliderVM
    {
        public IEnumerable<Slider> Sliders { get; set; }
        public SliderInfo SliderInfo { get; set; }

    }
}
