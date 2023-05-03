using EntityFramework_Slider.Data;
using EntityFramework_Slider.Helpers;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class SliderInfoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;      // bu interface vasitesi ile biz gedib WWw.root un icine cata bilecik
        public SliderInfoController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<SliderInfo> slidersInfo = await _context.SliderInfos.ToListAsync();

            return View(slidersInfo);

        }



        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();
            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo is null) return NotFound();
           
            return View(sliderInfo);
        }


        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/ //sadece indexe gedir hansiki inputa data elavve edib category yaradacaq
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(SliderInfo sliderInfo)
        {
            try
            {

                if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                {
                    return View();    // lahiyenin ustune vurub arxa fonda null ucun olan  baglmaq lazimdi  yeni   <Nullable>disable</Nullable> elemek lazimdir 
                }

                if (!sliderInfo.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View();
                }

                if (!sliderInfo.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View();
                }


                var existData = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Title.Trim().ToLower() == sliderInfo.Title.Trim().ToLower());
                // yoxlayiriq bize gelen yeni inputa yazilan name databazada varsa  error chixartmaq uchun

                if (existData is not null) /*gelen data databazamizda varsa yeni null deyilse*/
                {
                    ModelState.AddModelError("Title", "This data already exist!"); // bu inputa name daxil etmeyende error chixartsin. Name propertisinin altinda. buradaki Name hemin input-un adidir.

                    return View();
                }

                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 


                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                await FileHelper.SaveFlieAsync(path, sliderInfo.Photo);

                sliderInfo.SignatureImage = fileName;

                await _context.SliderInfos.AddAsync(sliderInfo);
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

                SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (sliderInfo is null) return NotFound();

             

                string path = FileHelper.GetFilePath(_env.WebRootPath, "img", sliderInfo.SignatureImage);

                FileHelper.DeleteFile(path);

                _context.SliderInfos.Remove(sliderInfo);

                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index));

                return Ok();

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

            SliderInfo sliderInfo = await _context.SliderInfos.FirstOrDefaultAsync(m => m.Id == id);

            if (sliderInfo is null) return NotFound();

            return View(sliderInfo);

        }



        [HttpPost]
      
        public async Task<IActionResult> Edit(int? id, SliderInfo sliderInfo)
        {
            try
            {
                if (id == null) return BadRequest();

                SliderInfo dbSliderinfo = await _context.SliderInfos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbSliderinfo is null) return NotFound();


                if (!sliderInfo.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(dbSliderinfo);
                }

                if (!sliderInfo.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(dbSliderinfo);
                }

                string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "img", dbSliderinfo.SignatureImage);

                FileHelper.DeleteFile(oldPath);

                string fileName = Guid.NewGuid().ToString() + "_" + sliderInfo.Photo.FileName; // Guid.NewGuid() bu neynir bir id kimi dusune birerik hemise ferqli herifler verir mene ki men sekilin name qoyanda o ferqli olsun tostring ele deyirem yeni random oalraq ferlqi ferqli sekil adi gelecek  ve  slider.Photo.FileName; ordan gelen ada birslerdir 

                string newpath = FileHelper.GetFilePath(_env.WebRootPath, "img", fileName);

                //using (FileStream stream = new(newpath, FileMode.Create))    ///  using() neynir elaqeni qirir yeni bu www.rootun icinde getdi daha sonra deyiremki elaqeni qir bunu yazmasam exception cixacaq mutleq elaqeni qirlamiyam arxa planda qebz collectionu isleyir ve qirandan sonra silir arxa planda biz gormurk bunu
                //{
                //    await slider.Photo.CopyToAsync(stream);     // deyiremki yuxardaki yaratdiqimi   copy to ele streami  yeni icindeki path yeni fiziki olaraq lahiyemin icine www.rootun icine axi pathde yazmisam  kopyalayir atir ora upload edende sekli   // FileStream  bi neyinir fiziki olaraq kompyuterimde  file crate ede bilim deye bunu yazaliyam ve kansruktoru bizden data isdeyir  // FileMode ise ora ne edirikse tutaqki Createdise . crate yaziriq 
                //}

                await FileHelper.SaveFlieAsync(newpath, sliderInfo.Photo);

                sliderInfo.SignatureImage = fileName;

                _context.SliderInfos.Update(sliderInfo);

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
