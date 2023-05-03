using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;   
        // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik
        public ExpertController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Experts> experts = await _context.Experts.ToListAsync();
            return View(experts);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            
            Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

          
            if (expert is null) return NotFound();
      
            return View(expert);
        }


        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Experts experts)
        {
            try
            {

                if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                {
                    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                }


                if (!experts.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!experts.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }


                string fileName = Guid.NewGuid().ToString() + "_" + experts.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                await FileHelper.SaveFlieAsync(path, experts.Photo);

                experts.Image = fileName;

                await _context.Experts.AddAsync(experts);

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

            Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

            if (expert is null) return NotFound();

            return View(expert);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int? id, Experts expert)
        {
            try
            {
                if (id == null) return BadRequest();

                Experts dbexpert = await _context.Experts.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbexpert is null) return NotFound();


                if (!expert.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbexpert);
                }

                if (!expert.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbexpert);
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbexpert.Image);

                FileHelper.DeleteFile(oldPath);

                string fileName = Guid.NewGuid().ToString() + "_" + expert.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                string newpath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                //using (FileStream stream = new(newpath, FileMode.Create))    ///  using() neynir elaqeni qirir yeni bu www.rootun icinde getdi daha sonra deyiremki elaqeni qir bunu yazmasam exception cixacaq mutleq elaqeni qirlamiyam arxa planda qebz collectionu isleyir ve qirandan sonra silir arxa planda biz gormurk bunu
                //{
                //    await slider.Photo.CopyToAsync(stream);     // deyiremki yuxardaki yaratdiqimi   copy to ele streami  yeni icindeki path yeni fiziki olaraq lahiyemin icine www.rootun icine axi pathde yazmisam  kopyalayir atir ora upload edende sekli   // FileStream  bi neyinir fiziki olaraq kompyuterimde  file crate ede bilim deye bunu yazaliyam ve kansruktoru bizden data isdeyir  // FileMode ise ora ne edirikse tutaqki Createdise . crate yaziriq 
                //}

                await FileHelper.SaveFlieAsync(newpath, expert.Photo);

                expert.Image = fileName;

                _context.Experts.Update(expert);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)
            {

                return RedirectToAction("Error", new { msj = ex.Message }); // eror mesajimizi diger seyfeye yoneldir "Error" yazdiqimiz indexe yoneldir
            }


        }



        [HttpPost]
       
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                if (id == null) return BadRequest();

                Experts expert = await _context.Experts.FirstOrDefaultAsync(m => m.Id == id);

                if (expert is null) return NotFound();



                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", expert.Image);

                FileHelper.DeleteFile(path);

                _context.Experts.Remove(expert);

                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));

                return Ok();

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
