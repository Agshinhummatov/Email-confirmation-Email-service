using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface ISliderService
    {
        Task<IEnumerable<Slider>> GetAll();
        Task<SliderInfo> GetInfo();
    }
}
