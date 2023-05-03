using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Blog:BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Don't be empty")]
        public string Description { get; set; }
        public string Image { get; set; }


        [NotMapped, Required(ErrorMessage = "Don't be empty")]  // NotMapped add-migration edende  date base dusmur bunu yazanda 
        public IFormFile Photo { get; set; } // sistemin bize verdiyi bu tipdir file sekil ve s ilseyende bunu yazmaliyiq mutleq, eger bunu yaziriqsa mutleq gedib asp-for="Photo" yazaciqki sekilimiz upload ola bilsin
        public DateTime Date { get; set; }
    }
}
