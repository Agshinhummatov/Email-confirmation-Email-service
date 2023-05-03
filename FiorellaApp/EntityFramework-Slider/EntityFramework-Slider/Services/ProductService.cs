using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAsync()   // productlarin sayini tapmaq ucun yazdiqimiz methodur adinda Async vermisiki asixron olsun
        {
            return await _context.Products.CountAsync(); // CountAsync() ssitemin verdiyi methodur
        }

        public async Task<IEnumerable<Product>> GetAll() => await _context.Products.Include(m => m.Images).ToListAsync();
        public async Task<Product> GetById(int id) => await _context.Products.FindAsync(id);
        public async Task<Product> GetFullDataById(int id) => await _context.Products.Include(m => m.Images).Include(m => m.Category)?.FirstOrDefaultAsync(m => m.Id == id); // yoxlayiriq databazadaki productun idisinnen cookiedeki productun(yeni basketdeki productun) idsi eynidirse

        public async Task<List<Product>> GetPaginatedDatas( int page , int take )  //bu method seyfeye uygun olaraq (int page ) data bazadan (int take) qeder datani gpturub gelir 
        {
           return  await _context.Products.Include(m=>m.Category).Include(m=>m.Images).Skip((page*take) - take).Take(take).ToListAsync();  // (page*take) - take)  vurub taki cixirkiqki hemise  skipe elediyim qeder onu gosdersin novebetilerde nedise diger seyfede onu gosdersin yeni bir seyfede 5 product geldi digerinde 6 dan baslasin 10 kimi gosdersin bu mentiqde 
        }
    }
}
