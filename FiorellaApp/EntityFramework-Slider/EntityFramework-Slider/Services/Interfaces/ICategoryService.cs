using EntityFramework_Slider.Models;
using System.Collections.Generic;

namespace EntityFramework_Slider.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();


        Task<List<Category>> GetPaginatedDatas(int page, int take);

         Task<int> GetCountAsync();
    }
}
