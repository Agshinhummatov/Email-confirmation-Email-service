using Elearn_temp.Data;
using Elearn_temp.Models;
using Elearn_temp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Elearn_temp.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAll() => await _context.Courses.Include(m => m.CourseImages).ToListAsync();


        public async Task<Course> GetById(int id) => await _context.Courses.FindAsync(id);


        public Task<int> GetCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Course> GetFullDataById(int id) => await _context.Courses.Include(m => m.CourseImages).Include(m => m.Author)?.FirstOrDefaultAsync(m => m.Id == id);


        public Task<List<Course>> GetPaginatedDatas(int page, int take)
        {
            throw new NotImplementedException();
        }
    }
}
