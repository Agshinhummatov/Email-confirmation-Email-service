using EntityFramework_Slider.Data;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Services
{
    public class LayoutService: ILayoutService   //layputService ni Layoutumuza nese elave gondere bilmek uchun yaradiriq.
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;

        public LayoutService(AppDbContext context ,
                             IBasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

         public LayoutVM GetSettingDatas()  //keyine valuesine gore datalari elde etmek  uchun method
         {
            Dictionary<string,string> settings = _context.Settings.AsEnumerable().ToDictionary(m => m.Key, m => m.Value);
            // dictionary ye chevirmek uchun evveclce AsEnumrable().Todictionary() edirik key ve value sine gore. yoxsa dictionary e chevire bilmirik. bazada key value qoydugumuza gore dictionary e chevire bilirik.
            // datalari todictionary edirikse qanunu olaraq AsEnumrable etmeliyik.


            List<BasketVM> basketDatas = _basketService.GetBasketDatas();


            //int count =  basketDatas.Sum(b => b.Count);   //basketdeki datalarinin cemini hesabla her productun countuna gore
            //LayoutVM model = new()
            //{
            //    Settings = settings,
            //    BasketCount = count          //bunlari qisaldiriq deye istifade etmirik
            //};

            return new LayoutVM { Settings = settings, BasketCount = basketDatas.Sum(b => b.Count) };

         }



    }
}
