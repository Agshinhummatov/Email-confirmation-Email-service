using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class SliderInfo:BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Don't be empty")]
        public string Description { get; set; }

        public string SignatureImage { get; set; }

        [NotMapped, Required(ErrorMessage = "Don't be empty")]  
        public IFormFile Photo { get; set; }
    }
}
