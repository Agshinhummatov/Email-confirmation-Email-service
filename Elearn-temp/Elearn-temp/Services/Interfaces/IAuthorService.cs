using Elearn_temp.Models;

namespace Elearn_temp.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAll();


        Task<List<Author>> GetPaginatedDatas(int page, int take);

        Task<int> GetCountAsync();

    }
}
