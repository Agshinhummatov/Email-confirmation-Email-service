using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class FooterService : IFooterService
    {
        private readonly AppDbContext _context;
        public FooterService(AppDbContext context)
        {
            _context = context;
        }
        public Dictionary<string, string> GetSettingDatas()
        {
            return _context.Settings.AsEnumerable().ToDictionary(m => m.Key, m => m.Value);
        }

        public async Task<IEnumerable<Social>> GetSocials()
        {
            return await _context.Socials.ToListAsync();

        }
    }
}
