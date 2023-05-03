using EntityFramework_Slider.Areas.Admin.ViewModels;
using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        private readonly ICategoryService _categoryService;

        private readonly IWebHostEnvironment _env;      // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik

        private readonly AppDbContext _context;
        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env, AppDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _context = context;
        }

        public async Task<IActionResult> Index( int page = 1, int take = 5)  // page = 1 deafult deyer veriemki her defe acilanda 1 ci gosdersin 1 ci page gosdersin
        {
            List<Product> products = await _productService.GetPaginatedDatas(page,take); //page ve take gonderirik icine hemin methoda yazilibdi Servicde orda qebul edecik 

            List<ProdcutListVM> mappedDatas = GetMappedDatas(products); // datalari getirir mene

            int pageCount = await GetPageCountAsync(take); //paglerin sayin gosderir methodu asaqida yazmisiq 

            Paginate<ProdcutListVM> paginatedDatas = new(mappedDatas, page, pageCount);  /// methodumuz bir generice cixartmisiq Paginate bunda her yerde istifade edecik methoda bizden 1 ci datani isdeyir mappedDatas, 2 ci page yeni curet page  3 cu ise totalPage paglerin sayini gosderen methodu gonderirik icine

            ViewBag.take = take;

            return View(paginatedDatas);
        }

        //paglerin sayini veren method

        private async Task<int> GetPageCountAsync(int take)
        {
            var productCount = await _productService.GetCountAsync();  // bu methoda mene productlarin countunu verir
            return (int)Math.Ceiling((decimal)productCount / take);     /// burda bolurki  product conutumzun nece dene take edirikse o qederde gosdersin yeni asqqidaki 1 2 3 yazir onlarin sayini tapmaq ucun 

            //Math.Ceiling() methodu bizden decimal isdeyir bu neynir tutaqki geldi 3.5 eledi bunu yuvarlasdirsin 4 elesin (int)Math ise biz decimal yazmisiq methdmuzun tipi int di ona casstitng elesin

        }

        // pasingation method 
        private List<ProdcutListVM> GetMappedDatas(List<Product> products)
        {
            List<ProdcutListVM> mappedDatas = new();

            foreach (var product in products)
            {
                ProdcutListVM prodcutVM = new()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Count = product.Count,
                    CategoryName = product.Category.Name,
                    MainImage = product.Images.Where(m => m.IsMain).FirstOrDefault()?.Image


                };

                mappedDatas.Add(prodcutVM);

            }


            return mappedDatas;

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //IEnumerable<Category> categories = await _categoryService.GetAll();

            //ViewBag.categories = new SelectList(categories, "Id", "Name"); /// bu neynir gedir selectin icindeki Id sini goturur ve nameini getirir mene id gedecek selectin valusuna namde gedecek textine // textine gorede id sini gondere bilecik

            ViewBag.categories = await GetCategoriesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdcutCreateVM model)
        {
            try
            {
                //IEnumerable<Category> categories = await _categoryService.GetAll();

                //ViewBag.categories = new SelectList(categories, "Id", "Name");  //httpPost methodunda yazirkiqki frumuz submit olanda yeni refresh olanda hemin seletimiz ordan getmesin view bag ile gonderirik datani


                 ViewBag.categories = await GetCategoriesAsync();

                if (!ModelState.IsValid)
                {
                    return View(model); //  is validi yoxlayirki bos olmasin ve view icine bize gelen model  gonderiki eger biri sehv olarsa inputlari bos saxlamasin
                }


                foreach (var photo in model.Photos)
                {

                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }



                }

                List<ProductImage> productImages = new();  // list yaradiriq  burda hemin listada asqi methoda add edecik imagleri

                foreach (var photo in model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 


                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFlieAsync(path, photo);


                    ProductImage productImage = new()   // bir bir sekileri goturur forech icinde
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage); // yuxardaki  List<ProductImage> add edir sekileri yeni nece dene sekili varsa o qederde add edecek

                }

                productImages.FirstOrDefault().IsMain = true; // bu neynir elimizdeki list var icinde imagler var gelir onlardan biricsin defaltunu ture edirki productlarda 1 ci sekili gosdersin

                decimal convertedPrice = decimal.Parse(model.Price); // deyiremki mene gelen productun qiymetini inputa noqte ile daxil edirler yeni 25.50 ni cevir  string gelir axi menimde esas productumda yeni data bazamda decimaldi gel sen onu parce dele decimala

                Product newProduct = new()
                {
                    Name = model.Name,       // name price count description categoryId Images bunlarin hamsi Productun icindaki modelerimdir 
                    Price = convertedPrice, // burda ise men yuxarda replace eledim pirceki noteqin yerine data bazaya vergul kimi dusun 
                    Count = model.Count,         // model.price model.count mmodel.Description , model.CategoryId bunlar ise mene gelen yuxardaki  methodun icindeki model lerimdir yeni bu  public async Task<IActionResult> Create(ProdcutCreateVM model) burdaki model
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Images = productImages  // bu ise yuxarda yidgim listin icina imagleri forech saldim  
                };

                await _context.ProductImages.AddRangeAsync(productImages); // AddRangeAsync bu method bize listi yigir add edir 
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            } 
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();
            
            Product product = await _productService.GetFullDataById((int)id);

            if (product == null) return NotFound();

            ViewBag.desc = Regex.Replace(product.Description, "<.*?>", String.Empty);  // bunu biz decriptonun icinde ckeedirtor qosmusuq buda viwe gelende inputun icinde Html.raw qoymaq olmur bizde bunu yaziriqki ordaki tagleri silsin ancaq icini gosdersin

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]     // bu bize delete methodun view da delete click ende bu methoda gelsin deyedi
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _productService.GetFullDataById((int)id);

            if (product is null) return NotFound();


            foreach (var item in product.Images)
            {

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);

                FileHelper.DeleteFile(path);
            }


            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct is null) return NotFound();

            ViewBag.categories = await GetCategoriesAsync();

            //string convertedPrice = dbProduct.Price.ToString();


            return View(new ProductUpdateVM
            {
               
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price.ToString("0.#####").Replace(",","."),
                Count = dbProduct.Count,
                CategoryId = dbProduct.CategoryId,
                Images = dbProduct.Images
            });

        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ProductUpdateVM updatedProduct)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (!ModelState.IsValid) return View(updatedProduct);

            Product dbProduct = await _productService.GetFullDataById((int)id);

            if (dbProduct is null) return NotFound();

            if (updatedProduct.Photos != null)
            {

                foreach (var photo in updatedProduct.Photos)
                {

                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View(dbProduct);
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View(dbProduct);
                    }



                }

                foreach (var item in dbProduct.Images)
                {

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", item.Image);

                    FileHelper.DeleteFile(path);
                }



                List<ProductImage> productImages = new();  // list yaradiriq  burda hemin listada asqi methoda add edecik imagleri

                foreach (var photo in updatedProduct.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 


                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    await FileHelper.SaveFlieAsync(path, photo);


                    ProductImage productImage = new()   // bir bir sekileri goturur forech icinde
                    {
                        Image = fileName
                    };

                    productImages.Add(productImage); // yuxardaki  List<ProductImage> add edir sekileri yeni nece dene sekili varsa o qederde add edecek

                }

                productImages.FirstOrDefault().IsMain = true;

                dbProduct.Images = productImages;
            }

            decimal convertedPrice = decimal.Parse(updatedProduct.Price);

            dbProduct.Name = updatedProduct.Name;
            dbProduct.Description = updatedProduct.Description;
            dbProduct.Price = convertedPrice;
            dbProduct.CategoryId = updatedProduct.CategoryId;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            ViewBag.categories = await GetCategoriesAsync();

            Product dbProduct = await _productService.GetFullDataById((int)id); ;

            ViewBag.desc = Regex.Replace(dbProduct.Description, "<.*?>", String.Empty);

            return View(new ProductUpdateVM   // view gonderirik bunlari 
            {
                
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price.ToString("0.#####").Replace(",", "."),
                Count = dbProduct.Count,
                CategoryId = dbProduct.CategoryId,
                Images = dbProduct.Images,
                CategoryName = dbProduct.Category.Name
            });
        }





        // view bag ile gonderceyimiz method selectleri verir bize categorey id ve nameni 
        private async Task<SelectList> GetCategoriesAsync()  
        {

            IEnumerable<Category> categories = await _categoryService.GetAll();

            return  new SelectList(categories, "Id", "Name"); /// bu neynir gedir selectin icindeki Id sini goturur ve nameini getirir mene id gedecek selectin valusuna namde gedecek textine // textine gorede id sini gondere bilecik


        }



    }
}
