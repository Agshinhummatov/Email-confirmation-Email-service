using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertHeaderController : Controller
    {



        private readonly IExpertService _expertService;
        private readonly AppDbContext _context;
        public ExpertHeaderController(IExpertService expertService, AppDbContext context)
        {
            _expertService = expertService;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            return View(await _expertService.GetHeader());

        }



        [HttpGet]
        public IActionResult Create()     /*async-elemirik cunku data gelmir databazadan*/ //sadece indexe gedir hansiki inputa data elavve edib category yaradacaq
        {

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpertsHeader expertsHeader)     // bazaya neyise save edirik deye asinxron olmalidi
        {
            try
            {
                //if (!ModelState.IsValid)    // create eden zaman input null olarsa, yeni isvalid deyilse(isvalid olduqda data daxil edilir) view a qayit
                //{
                //    return View();
                //}

                var existData = await _context.ExpertsHeaders.FirstOrDefaultAsync(m => m.Title.Trim().ToLower() == expertsHeader.Title.Trim().ToLower() && m.Description.Trim().ToLower() == expertsHeader.Description.Trim().ToLower());
                // yoxlayiriq bize gelen yeni inputa yazilan name databazada varsa  error chixartmaq uchun

                if (existData is not null) /*gelen data databazamizda varsa yeni null deyilse*/
                {
                    ModelState.AddModelError("Name", "This data already exist!"); // bu inputa name daxil etmeyende error chixartsin. Name propertisinin altinda. buradaki Name hemin input-un adidir.

                    return View();
                }


                 _context.ExpertsHeaders.AddAsync(expertsHeader);  //bazadaki categorie tablesine category ni add edir liste
                await _context.SaveChangesAsync();    //save edir bazaya 
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  // category find edir yeni datani tapir 

            if (expertsHeader is null) return NotFound(); // find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            _context.ExpertsHeaders.Remove(expertsHeader); // remove ele elimdeki categoryani
            await _context.SaveChangesAsync();  // daha sonra data bazaya save et
            return RedirectToAction(nameof(Index)); // indexsine gonder delete edenden sonra


        }




        [HttpGet]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


           ExpertsHeader  expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  // category find edir

            if (expertsHeader is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            return View(expertsHeader);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, ExpertsHeader expertsHeader)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            ExpertsHeader dbexpertsHeade = await _context.ExpertsHeaders.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id); // category find edir AsNoTracking() bunu ona gore yaziramki data base ile elaqeni qirsin yeni bu gelir update ede bilmir bunu yazanda  ise update edir

            if (dbexpertsHeade is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu

            if (dbexpertsHeade.Title.Trim().ToLower() == expertsHeader.Title.Trim().ToLower() && dbexpertsHeade.Description.Trim().ToLower() == expertsHeader.Description.Trim().ToLower()) /// bu yoxlayirki mene gelen name data bazadaki name beraberdise sen bunu data base requset gonderme
            {
                return RedirectToAction(nameof(Index));   //ifin icine girir ve dayandirir methodu indexine retrun edir
            }

            /* dbcategory.Name = category.Name;  */ //   dbcategory.Name data bazamdki name mene gelen nemae beraber ele  category.Name  methoda viewdan gonderirik submit edende

            _context.ExpertsHeaders.Update(expertsHeader);  // buda ona gore yaziriqki biz  dbcategory.Name = category.Name bundan istifade etmeyek tutaqki 4 dene input geldi gelib bir bir beraberlesdirmeliyem amma bunu yazanda ehtiyyac yoxdur

            await _context.SaveChangesAsync(); //data bazaya save edirik

            return RedirectToAction(nameof(Index));

        }



        [HttpGet]

        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest(); // id nulldusa eger retrun ele badRequest'i


            ExpertsHeader expertsHeader = await _context.ExpertsHeaders.FindAsync(id);  // category find edir

            if (expertsHeader is null) return NotFound(); /// find edenden sonra yoxla bele bir categrya yoxdusa retrun edir note fondu


            return View(expertsHeader);

        }

    }
}
