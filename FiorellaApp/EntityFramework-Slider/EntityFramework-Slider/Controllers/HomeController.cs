using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services;
using EntityFramework_Slider.Services.Interfaces;
using EntityFramework_Slider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EntityFramework_Slider.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISliderService _sliderService;
        private readonly IExpertService _expertService;
        private readonly IFooterService _footerService;
        //private readonly IBlogService _blogService;








        //private readonly IConfiguration _configuration;
        public HomeController(AppDbContext context, 
                              IBasketService basketService,
                              IProductService productService 
                              /*, IConfiguration configuration*/,
                              ICategoryService categoryService,
                              ISliderService sliderService,
                              IExpertService expertService,
                              IFooterService footerService
                             /* IBlogService blogService*/)
        {
            _context = context;
            _basketService = basketService;
            _productService = productService;
            _categoryService = categoryService;
            _sliderService = sliderService;
            _expertService = expertService;
            _footerService = footerService;
            //_configuration = configuration;
            //_blogService = blogService;
        }

        //public IActionResult Test()
        //{
        //    var user = _configuration.GetSection("Login:User").Value;
        //    var mail = _configuration.GetSection("Login:Mail").Value;
        //    return Content($"{user} {mail}");

        //}







        public async Task<IActionResult> Index()
        {


            //---------------- SESSION, COOKIE STORAGE start-----------------***

            //HttpContext.Session.SetString("name", "Pervin");   // session storageye datani qoymaq

            //Response.Cookies.Append("surname", "Rehimli", new CookieOptions { MaxAge = TimeSpan.FromMinutes(30)});  // cookie storageye datani qoymaq


            //Book book = new Book
            //{
            //    Id = 1,
            //    Name = "Xoshrov ve Shirin"
            //};

            //objecti cookie ye qoymaq uchun Json a chevirib(yeni string formatina chevirib) qoya bilerik. bunu uchun convert serialize edirik.
            //Response.Cookies.Append("book",JsonConvert.SerializeObject(book));

            //---------------- SESSION, COOKIE STORAGE end-----------------***





            //bu yazdiqlarimiz linkue queryler adlanir.bu kodlar vasitesile lazim  olan iwleri gorursen
            IEnumerable<Slider> sliders = await _sliderService.GetAll();

            //IQueryable<Slider> slide = _context.Sliders.AsQueryable();
            //List<Slider> query = slide.Where(m=> m.Id > 5).ToList();
            //Iqueryable ile query yaradir ramda saxlayiriq, hele data getirmirik,
            //sonra werte uygun datani getiririk.
            //ToList() yazdiqda request gedir data bazaya. yazmadiqda getmir.

            SliderInfo sliderInfo = await _sliderService.GetInfo();



            //Listin ferqli metodlari var.Add,Remove ve s. Listi controllerde hazirlayob view a gonderirik.
            //Listin ichinde olan elave metodlarda iwleyir.
            //IEnumerable da ise hech bir metod yoxdur, ancaq dovre sala bilirsen.View da datani Inumerable kimi qebul etmek daha yaxwidir.
            //Viewda datani sadece foreach edib gosteririk deye, Inumerable iwletmek yaxwidir.LIstin elave metodlari iwleyib sistemi agirlawdirmasin deye.
            IEnumerable<Category> categories = await _categoryService.GetAll();


            //burada wekil null gelir Ona gore firstOrDefault ede bilmir.Ona burada Whereden evvel include edirik.
            //Burada relationlu tablelerin icherisinde, ichinde olan o biri tablenide (meselen productun ichinde yazdigimiz  productImage)yeni, productImageni productnan birlikde getirmek isteyirikse,
            //Products.Include edirik
            IEnumerable<Product> products = await _productService.GetAll();   //butun productlari basketden goturmek uchun

            About abouts = await _context.Abouts.Include(m => m.Adventages).FirstOrDefaultAsync();

            IEnumerable<Experts> experts = await _expertService.GetAll();

            IEnumerable<ExpertsHeader> expertsHeader = await _expertService.GetHeader();

            Subscribe subscribe = await _context.Subscribes.FirstOrDefaultAsync();

            //OurBlog ourBlog = await _blogService.GetOurBlogHeader();

            IEnumerable<Say> says = await _context.Says.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Instagram> instagrams = await _context.Instagrams.Where(m => m.SoftDelete == false).ToListAsync();

            IEnumerable<Social> socials = await _footerService.GetSocials();





            HomeVM model = new()
            {
                Categories = categories,
                Products = products,
                Abouts = abouts,
                Subscribe = subscribe,
                Says = says,
                //OurBlog = ourBlog,
                Instagrams = instagrams,
            };

            return View(model);
        }



        #region Session/ Cookie storage[storagelerden datani goturmek]


        //            //---------------- SESSION, COOKIE STORAGE start-----------------***


        //public IActionResult Test()   //storageden datani goturub UI da gostermek uchun metod
        //{
        //    var sessionData = HttpContext.Session.GetString("name"); // session storageden datani gotumek uchun

        //    var cookieData = Request.Cookies["surname"];    // cookie storageden datani goturmek

        //    var objectData = JsonConvert.DeserializeObject<Book>(Request.Cookies["book"]);   //object formatinda datani cookie den goturmek. 
        //    //bunu uchun desrialize edirik(yeni json(string) formatini objecte cheviririk ).


        //    return Json(sessionData);
        //}

        //              //---------------- SESSION, COOKIE STORAGE end-----------------***

        #endregion



        #region Basket mentiqi / cookie sorage / uzun yazilish

        //[HttpPost] //yeni data elave edende yoxlayir ki token heqiqeten saytdan girilende yaranib eks halda metoda girmeyecek
        //[ValidateAntiForgeryToken]  // hemin tokeni yoxlayan atribut budur.
        //public async Task<IActionResult> AddBasket(int? id)
        //{
        //    if(id == null) return BadRequest(); 

        //    Product dbproduct = await _context.Products.FindAsync(id);   //data bazadan productlarimizin ichinden id- sine gore product goturmek

        //    if (dbproduct == null) return NotFound();


        //    List<BasketVM> basket;  //bosh bir list yaradiriq

        //    if (Request.Cookies["basket"] != null) //cookie deki basket null deyilse
        //    {
        //        basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);    // eger cookide data varsa elimizde olan datani beraber edirik bize gelen teze dataya. bunun uchun ge
        //        //bunu Listimize deserialize edirikki tipleri beraber olsun
        //        //eyer coockide data varsa yani null deyilse coockide olan datani goturub = DeserializeObject<List<BasketVM>>.esayn edirik elmizde olan List<BasketVM>e
        //    }
        //    else
        //    {
        //        basket = new List<BasketVM>();
        //        //data yoxdursa teze liist yaradir
        //    }


        //    BasketVM? existProduct = basket?.FirstOrDefault(m => m.Id == dbproduct.Id);
        //    //yoxlayiriqki basketin ichinde gelen id var ya yox. varsa basketimiz exist olur. yoxdursa null olur


        //    if (existProduct == null) //baskextde existProduct nulldirsa(yeni elave etdiyimiz productdan basketde yoxdursa yenisini elave edir. varsa else de sayini artirir)
        //    {
        //        basket?.Add(new BasketVM   //listimize yeni data elave edirik
        //        {
        //            Id = dbproduct.Id,
        //            Count = 1
        //        });
        //    }
        //    else
        //    {
        //        existProduct.Count++;
        //    }

        //   //yuxaridakilari odedikden sonra add edir cookie ye

        //    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        //    //elimizde olan listimizi response kimi append edirik cookieye


        //    return RedirectToAction(nameof(Index)); // bawqa actiona yonlendirmek uchun(sehife refrsh olacaq amma)
        //}

        //---------------- BASKET mentiqi,  COOKIE STORAGE end -----------------***


        #endregion







        //---------------- BASKET mentiqi,  COOKIE STORAGE start [ metodlar ile qisa yaziliwi ]-----------------***

        [HttpPost] //yeni data elave edende yoxlayir ki token heqiqeten saytdan girilende yaranib eks halda metoda girmeyecek
        /*   [ValidateAntiForgeryToken] */ // hemin tokeni yoxlayan atribut budur.




        //Ajaxla atilan reguest buraya gelir
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product dbproduct = await _productService.GetById((int)id);     //data bazadan productlarimizin ichinden id- sine gore product goturmek  (id null gele biler deye int-e cast edirik)

            if (dbproduct == null) return NotFound();

            List<BasketVM> basket = _basketService.GetBasketDatas(); //bosh bir list yaradib elimizdeki metoda(basketdeki datalari goturduyumuz) beraber edirik.

            BasketVM existProduct = basket.FirstOrDefault(m => m.Id == dbproduct.Id);
            //yoxlayiriqki basketin ichinde databazadan gelen id-li product var ya yox. varsa basketimizdeki product exist olur. yoxdursa null olur

            _basketService.AddProductToBasket(existProduct, dbproduct, basket);  // baskete datalari elave etmek uchun method.
                                                                  // icherisine yuxarida olan  existProductu(yeni basketde olan), databazadan id e gore yoxlayib goturduyumuz productu, elimizde olan basket listini gonderirik.




            int basketCount = basket.Sum(m => m.Count);  // basketdeki datalarin countunu tapib variableye beraber edirik
                                                         //her add ednde countu gondermek uchun elmizde olan basketdeki datanin countunu ajax a gonderirik


            /*  return RedirectToAction(nameof(Index));*/ // bawqa actiona yonlendirmek uchun(sehife refrsh olacaq amma). 
            return Ok(basketCount);
        }



   


 

     


    }


    //class Book   // object weklinde datani cookie qoymaq uchun class yaratdiq
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }

    //}
}