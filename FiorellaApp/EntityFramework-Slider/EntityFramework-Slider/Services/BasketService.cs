using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Newtonsoft.Json;

namespace EntityFramework_Slider.Services
{
    public class BasketService : IBasketService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;  //bunu qeyd edirikki brauserde olan datalara(yeni cookie ye ) chata bilmek uchun. controllerler brauserle elaqeli oldgu uchun rahat chata bilir.
                                                                     //serviceler brauserde olan datalara chata bilmir deye  IHttpContextAccessor _httpContextAccessor; bunu qeyd edirik.

        public BasketService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddProductToBasket(BasketVM existProduct, Product product, List<BasketVM> basket)
        {
            if (existProduct == null) //baskextde existProduct nulldirsa(yeni elave etdiyimiz productdan basketde yoxdursa yenisini elave edir. varsa else de sayini artirir)
            {
                basket?.Add(new BasketVM   //listimize yeni data elave edirik
                {
                    Id = product.Id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }

            //yuxaridakilari odedikden sonra add edir cookie ye

            _httpContextAccessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
            //elimizde olan listimizi response kimi append edirik cookieye
        }

        public void DeleteProductFromBasket(int id)
        {
            List<BasketVM> basketProducts = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContextAccessor.HttpContext.Request.Cookies["basket"]); // basketde olan productlarimizi gotururuk

            BasketVM deleteProduct = basketProducts.FirstOrDefault(m => m.Id == id);  // basket productun ichinden delete olunan idli productu tapiriq

            basketProducts.Remove(deleteProduct); // sonra basketdeki productlarimizin ichinden remove edirik silmek isteyeceyimiz  productu

            _httpContextAccessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketProducts)); // sonra cookie ni(basketi) update edirik. yeniden elimizde olan listi elave edirik cookie ye
        }



        public List<BasketVM> GetBasketDatas()
        {
            List<BasketVM> basket;  //bosh bir list yaradiriq

            var data = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            if (_httpContextAccessor.HttpContext.Request.Cookies["basket"] != null) //cookie deki basket null deyilse
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(_httpContextAccessor.HttpContext.Request.Cookies["basket"]);    // eger cookide data varsa elimizde olan datani beraber edirik bize gelen teze dataya. bunun uchun ge
                //bunu Listimize deserialize edirikki tipleri beraber olsun
                //eyer coockide data varsa yani null deyilse coockide olan datani goturub = DeserializeObject<List<BasketVM>>.esayn edirik elmizde olan List<BasketVM>e
            }
            else
            {
                basket = new List<BasketVM>();
                //data yoxdursa teze liist yaradir
            }
            return basket;
        }
    }
}
