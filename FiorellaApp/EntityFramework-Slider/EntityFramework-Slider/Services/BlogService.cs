using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _context;
        public BlogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAll()
        {
            return await _context.Blogs.ToListAsync();
        }

        public async Task<OurBlog> GetOurBlogHeader()
        {
            return await _context.OurBlogs.FirstOrDefaultAsync();
        }
    }
}
