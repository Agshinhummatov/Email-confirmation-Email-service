using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class ProdcutCreateVM
    {
        [Required(ErrorMessage = "Don't be empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Don't be empty")]
        public string Price { get; set; }                // decimal yazmiriq string yazaciqki.
        [Required(ErrorMessage = "Don't be empty")]
        public int Count { get; set; }

        [Required(ErrorMessage = "Don't be empty")]
        public string Description { get; set; }

        public int CategoryId { get; set; }  // category id ni verecek bize

        [Required(ErrorMessage = "Don't be empty")]
        public List<IFormFile> Photos { get; set; } // list yazirkiqki coxlu sekileri ola biler  add edende productun

    }
}
