using Elearn_temp.Data;
using Elearn_temp.Models;
using Elearn_temp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Elearn_temp.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;
        public AuthorService(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IEnumerable<Author>> GetAll()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Authors.CountAsync();
        }

        public Task<List<Author>> GetPaginatedDatas(int page, int take)
        {
            throw new NotImplementedException();
        }
    }
}
