using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class SliderService : ISliderService
    {

        private readonly AppDbContext _context;
        public SliderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Slider>> GetAll()
        {
            return await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();
        }

        public async Task<SliderInfo> GetInfo()
        {
            return await _context.SliderInfos.OrderByDescending(m => m.Id).FirstOrDefaultAsync();
        }
    }
}
