using EntityFramework_Slider.Models;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface IExpertService
    {
        Task<IEnumerable<Experts>> GetAll();

        Task<IEnumerable<ExpertsHeader>> GetHeader();
    }
}
