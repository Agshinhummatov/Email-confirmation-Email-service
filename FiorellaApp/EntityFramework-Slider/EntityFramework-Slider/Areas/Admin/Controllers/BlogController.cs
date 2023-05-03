using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;      // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik

        public BlogController(AppDbContext context , IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _context.Blogs.Where(m => !m.SoftDelete).ToListAsync();
            return View(blogs);
        }




        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            //badrequest routingde falan nese sehvlik olduqda chixan exseptiondur
            //meselen routingden sehven axtariw edilen id silinerse bu zaman chixan errordur



            //gelen id li elementi bazadan tapmaq uchun edirik bunu
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);


            //eyer kimse sehv regem yazibsa Url-e
            //NotFound-tapilmadi deye Exception 
            if (blog is null) return NotFound();
            //gelen id bazamizda yoxdursa chican error

            return View(blog);
        }



        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/
        {

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Blog blog)
        {
            try
            {

                if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                {
                    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                }


                if (!blog.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!blog.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }


                var existData = await _context.Blogs.FirstOrDefaultAsync(m => m.Header.Trim().ToLower() == blog.Header.Trim().ToLower());
                // yoxlayiriq bize gelen yeni inputa yazilan name databazada varsa  error chixartmaq uchun

                if (existData is not null) /*gelen data databazamizda varsa yeni null deyilse*/
                {
                    ModelState.AddModelError("Header", "This data already exist!"); // bu inputa Header  daxil etmeyende error chixartsin. Name propertisinin altinda. buradaki Name hemin input-un adidir.

                    return View();
                }



                string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                /*   string path = Path.Combine(_env.WebRootPath, "img", fileName); */  // Path.Combine nedir www.root kimi olan yoldur _env.WebRootPath bi www.rootun icindeyem hansi file qoyacam "img" file qoyacam demekdi  ve fileName yeni adini // path hara qoyduqmu gosderir mene yeni www.rootun icinde img folderin icine get yuxaride yazdiqim file name qoy yeni adini 

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                using (FileStream stream = new(path, FileMode.Create))    ///  using() neynir elaqeni qirir yeni bu www.rootun icinde getdi daha sonra deyiremki elaqeni qir bunu yazmasam exception cixacaq mutleq elaqeni qirlamiyam arxa planda qebz collectionu isleyir ve qirandan sonra silir arxa planda biz gormurk bunu
                {
                    await blog.Photo.CopyToAsync(stream);     // deyiremki yuxardaki yaratdiqimi   copy to ele streami  yeni icindeki path yeni fiziki olaraq lahiyemin icine www.rootun icine axi pathde yazmisam  kopyalayir atir ora upload edende sekli   // FileStream  bi neyinir fiziki olaraq kompyuterimde  file crate ede bilim deye bunu yazaliyam ve kansruktoru bizden data isdeyir  // FileMode ise ora ne edirikse tutaqki Createdise . crate yaziriq 
                }

                blog.Image = fileName;

                await _context.Blogs.AddAsync(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));



            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }


        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                if (id == null) return BadRequest();

                Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (blog is null) return NotFound();

                //string path = Path.Combine(_env.WebRootPath, "img", slider.Image);     /// men path yolu ile yoxladim img folderin  icindeki slider.image  yeni  bu adda sekkil varmi ve tapir hemin sekli stiring kimi 

                //if (System.IO.File.Exists(path))  // bu yoxlayirki ele bir file var www.rootun icinde yeni lahiyemde //System.IO ise bir usingdir onu yazmasam admin panelde oxumur
                //{
                //    System.IO.File.Delete(path);   // burda ise tapdiqim file silirem yeni image www.root un icindeki image 
                //}


                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", blog.Image);

                FileHelper.DeleteFile(path);

                _context.Blogs.Remove(blog);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }



        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

            if (blog is null) return NotFound();

            return View(blog);

        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int? id, Blog blog)
        {


            try
            {
                //if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                //{
                //    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                //}



                    if (!blog.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View();
                    }

                    if (!blog.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 200kb");
                        return View();
                    }


                    string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                    /*   string path = Path.Combine(_env.WebRootPath, "img", fileName); */  // Path.Combine nedir www.root kimi olan yoldur _env.WebRootPath bi www.rootun icindeyem hansi file qoyacam "img" file qoyacam demekdi  ve fileName yeni adini // path hara qoyduqmu gosderir mene yeni www.rootun icinde img folderin icine get yuxaride yazdiqim file name qoy yeni adini 

                    Blog dbBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id); // AsNoTracking() bunu yazmasaq elaqeni qira bilmir


                    //if (dbBlog.Header.Trim().ToLower() == blog.Header.Trim().ToLower() && dbBlog.Description.Trim().ToLower() == blog.Description.Trim().ToLower()
                    //        && dbBlog.Photo == blog.Photo && dbBlog.Date == blog.Date) /// bu yoxlayirki mene gelen name data bazadaki name beraberdise sen bunu data base requset gonderme
                    //{
                    //    return RedirectToAction(nameof(Index));   //ifin icine girir ve dayandirir methodu indexine retrun edir
                    //}

                    string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                    using (FileStream stream = new(path, FileMode.Create))    ///  using() neynir elaqeni qirir yeni bu www.rootun icinde getdi daha sonra deyiremki elaqeni qir bunu yazmasam exception cixacaq mutleq elaqeni qirlamiyam arxa planda qebz collectionu isleyir ve qirandan sonra silir arxa planda biz gormurk bunu
                    {
                        await blog.Photo.CopyToAsync(stream);     // deyiremki yuxardaki yaratdiqimi   copy to ele streami  yeni icindeki path yeni fiziki olaraq lahiyemin icine www.rootun icine axi pathde yazmisam  kopyalayir atir ora upload edende sekli   // FileStream  bi neyinir fiziki olaraq kompyuterimde  file crate ede bilim deye bunu yazaliyam ve kansruktoru bizden data isdeyir  // FileMode ise ora ne edirikse tutaqki Createdise . crate yaziriq 
                    }

                    string dbPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbBlog.Image);

                    FileHelper.DeleteFile(dbPath);

                    blog.Image = fileName;

                    _context.Blogs.Update(blog);

                    await _context.SaveChangesAsync();

                

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }



        }






        public IActionResult Error(string msj) // RedirectToAction("Error", new { msj = ex.Message }); gonderdiyimiz parametiri qebul edirik
        {
            ViewBag.error = msj; //viewbag ile gonderirik datani erroun indexine orda qebul edecik
            return View();
        }




    }
}
