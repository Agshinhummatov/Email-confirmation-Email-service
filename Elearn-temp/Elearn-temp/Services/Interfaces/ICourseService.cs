using Elearn_temp.Models;

namespace Elearn_temp.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Course> GetById(int id);
        Task<IEnumerable<Course>> GetAll();

        Task<Course> GetFullDataById(int id);

        Task<List<Course>> GetPaginatedDatas(int page, int take);

        Task<int> GetCountAsync();
    }
}
