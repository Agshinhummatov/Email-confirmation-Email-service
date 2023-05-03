using Elearn_temp.Areas.Admin.ViewModels;
using Elearn_temp.Data;
using Elearn_temp.Helpers;
using Elearn_temp.Models;
using Elearn_temp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Elearn_temp.Areas.Admin.Controllers
{

    [Area("Admin")]

    public class CourseController : Controller
    {

        private readonly ICourseService _courseService;

        private readonly IAuthorService _authorService;

        private readonly IWebHostEnvironment _env;      // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik

        private readonly AppDbContext _context;
        public CourseController(ICourseService courseService, IAuthorService authorService, IWebHostEnvironment env, AppDbContext context)
        {
            _courseService = courseService;
            _authorService = authorService;
            _env = env;
            _context = context;
        }




        public async Task<IActionResult> Index()
        {
            IEnumerable<Course> courses = await _context.Courses.Include(m => m.CourseImages).Include(m => m.Author).Where(m => !m.SoftDelete).ToListAsync();

            return View(courses);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();


            Course dbCourses = await _context.Courses.Include(m => m.CourseImages).Include(m => m.Author).FirstOrDefaultAsync(m => m.Id == id);


            if (dbCourses is null) return NotFound();

            ViewBag.desc = Regex.Replace(dbCourses.Description, "<.*?>", String.Empty);

            return View(new CourseDetailVM   // view gonderirik bunlari 
            {

                Name = dbCourses.Name,
                Description = dbCourses.Description,
                Price = dbCourses.Price.ToString("0.#####").Replace(",", "."),
                CountSale = dbCourses.CountSale,
                AuthorId = dbCourses.AuthorId,
                Images = dbCourses.CourseImages,
                AuthorName = dbCourses.Author.Name
            });

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //IEnumerable<Category> categories = await _categoryService.GetAll();

            //ViewBag.categories = new SelectList(categories, "Id", "Name"); /// bu neynir gedir selectin icindeki Id sini goturur ve nameini getirir mene id gedecek selectin valusuna namde gedecek textine // textine gorede id sini gondere bilecik

            ViewBag.authories = await GetAuthoriesAsync();

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateVM model)
        {  
            try
            {
                //IEnumerable<Category> categories = await _categoryService.GetAll();

                //ViewBag.categories = new SelectList(categories, "Id", "Name");  //httpPost methodunda yazirkiqki frumuz submit olanda yeni refresh olanda hemin seletimiz ordan getmesin view bag ile gonderirik datani


                ViewBag.authories = await GetAuthoriesAsync();

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

                List<CourseImage> courseImages = new();  // list yaradiriq  burda hemin listada asqi methoda add edecik imagleri

                foreach (var photo in model.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 


                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);

                    await FileHelper.SaveFlieAsync(path, photo);


                    CourseImage courseImage = new()   // bir bir sekileri goturur forech icinde
                    {
                        Image = fileName
                    };

                    courseImages.Add(courseImage); // yuxardaki  List<ProductImage> add edir sekileri yeni nece dene sekili varsa o qederde add edecek

                }

                courseImages.FirstOrDefault().IsMain = true; // bu neynir elimizdeki list var icinde imagler var gelir onlardan biricsin defaltunu ture edirki productlarda 1 ci sekili gosdersin

                int convertedPrice = int.Parse(model.Price); // deyiremki mene gelen productun qiymetini inputa noqte ile daxil edirler yeni 25.50 ni cevir  string gelir axi menimde esas productumda yeni data bazamda decimaldi gel sen onu parce dele decimala

                Course newProduct = new()
                {
                    Name = model.Name,       // name price count description categoryId Images bunlarin hamsi Productun icindaki modelerimdir 
                    Price = convertedPrice, // burda ise men yuxarda replace eledim pirceki noteqin yerine data bazaya vergul kimi dusun 
                    CountSale = model.CountSale,         // model.price model.count mmodel.Description , model.CategoryId bunlar ise mene gelen yuxardaki  methodun icindeki model lerimdir yeni bu  public async Task<IActionResult> Create(ProdcutCreateVM model) burdaki model
                    Description = model.Description,
                    AuthorId = model.AuthorId,
                    CourseImages = courseImages  // bu ise yuxarda yidgim listin icina imagleri forech saldim  
                };

                await _context.CourseImages.AddRangeAsync(courseImages); // AddRangeAsync bu method bize listi yigir add edir 
                await _context.Courses.AddAsync(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();

                Course course = await _courseService.GetFullDataById((int)id);

                if (course is null) return NotFound();

                //string path = Path.Combine(_env.WebRootPath, "img", slider.Image);     /// men path yolu ile yoxladim img folderin  icindeki slider.image  yeni  bu adda sekkil varmi ve tapir hemin sekli stiring kimi 

                //if (System.IO.File.Exists(path))  // bu yoxlayirki ele bir file var www.rootun icinde yeni lahiyemde //System.IO ise bir usingdir onu yazmasam admin panelde oxumur
                //{
                //    System.IO.File.Delete(path);   // burda ise tapdiqim file silirem yeni image www.root un icindeki image 
                //}
                foreach (var item in course.CourseImages)
                {
                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", item.Image);

                    FileHelper.DeleteFile(path);
                }

                _context.Courses.Remove(course);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }



        }


        [HttpPost]
        public async Task<IActionResult> DeleteProductImage(int? id)
        {
            if (id == null) return BadRequest();

            bool result = false;

            CourseImage courseImage = await _context.CourseImages.Where(m => m.Id == id).FirstOrDefaultAsync();

            if (courseImage == null) return NotFound();

            var data = await _context.Courses.Include(m => m.CourseImages).FirstOrDefaultAsync(m => m.Id == courseImage.CoruseId);

            if (data.CourseImages.Count > 1)
            {
                string path = FileHelper.GetFilePath(_env.WebRootPath, "images", courseImage.Image);

                FileHelper.DeleteFile(path);

                _context.CourseImages.Remove(courseImage);

                await _context.SaveChangesAsync();

                result = true;
            }

            data.CourseImages.FirstOrDefault().IsMain = true;

            await _context.SaveChangesAsync();

            return Ok(result);

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null) return BadRequest();

            ViewBag.authories = await GetAuthoriesAsync();

            Course dbCourse = await _courseService.GetFullDataById((int)id);

            if (dbCourse == null) return NotFound();


            CourseEditVM model = new()
            {
                Id = dbCourse.Id,
                Name = dbCourse.Name,
                CountSale = dbCourse.CountSale,
                Price = dbCourse.Price,
                AuthorId = dbCourse.AuthorId,
                Images = dbCourse.CourseImages,
                Description = dbCourse.Description
            };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CourseEditVM updatedCourse)
        {
            if (id == null) return BadRequest();

            ViewBag.authories = await GetAuthoriesAsync();

            Course dbCourse = await _context.Courses.AsNoTracking().Include(m => m.CourseImages).Include(m => m.Author).FirstOrDefaultAsync(m => m.Id == id);

            if (dbCourse == null) return NotFound();

            if (!ModelState.IsValid)
            {
                updatedCourse.Images = dbCourse.CourseImages;
                return View(updatedCourse);
            }

            List<CourseImage> courseImages = new();

            if (updatedCourse.Photos is not null)
            {
                foreach (var photo in updatedCourse.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        updatedCourse.Images = dbCourse.CourseImages;
                        return View(updatedCourse);
                    }

                    if (!photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        updatedCourse.Images = dbCourse.CourseImages;
                        return View(updatedCourse);
                    }
                }



                foreach (var photo in updatedCourse.Photos)
                {
                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "images", fileName);

                    await FileHelper.SaveFlieAsync(path, photo);

                    CourseImage courseImage = new()
                    {
                        Image = fileName
                    };

                    courseImages.Add(courseImage);
                }

                await _context.CourseImages.AddRangeAsync(courseImages);
            }

            //decimal convertedPrice = decimal.Parse(updatedProduct.Price.Replace(".", ","));

            Course newCourse = new()
            {
                Id = dbCourse.Id,
                Name = updatedCourse.Name,
                Price = updatedCourse.Price,
                CountSale = updatedCourse.CountSale,
                Description = updatedCourse.Description,
                AuthorId = updatedCourse.AuthorId,
                CourseImages = courseImages.Count == 0 ? dbCourse.CourseImages : courseImages
            };


            _context.Courses.Update(newCourse);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




      





        private async Task<SelectList> GetAuthoriesAsync()
        {

            IEnumerable<Author> authores = await _authorService.GetAll();

            return new SelectList(authores, "Id", "Name"); /// bu neynir gedir selectin icindeki Id sini goturur ve nameini getirir mene id gedecek selectin valusuna namde gedecek textine // textine gorede id sini gondere bilecik


        }



    }
}
