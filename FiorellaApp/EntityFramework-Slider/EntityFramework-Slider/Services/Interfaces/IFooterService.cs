using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IFooterService
    {
        Task<IEnumerable<Social>> GetSocials();

        Dictionary<string, string> GetSettingDatas();
    }
}
