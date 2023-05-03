

using System.ComponentModel.DataAnnotations; // duzgun onau using elemek lazimdir

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
       

        [Required(ErrorMessage = "Don't be empty")]  //null olub olmamasini yoxlayir. bu atributu qoyuruqsa null gele bilmez. yeni category create edende name vacib yazilmalidir.
      /*  [StringLength(10, ErrorMessage = "The name length must be max 20 characters")] */ // bu ise inputun uzunluquduki ora 20 herf ve ya reqem daxil elemek yanindaki ise mesajidi  ve bmutleq gedib inputun icine  minlength="" maxlength="" yazmaliyiq 
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }  //categoryden yola chixanda producta chata bilmek uchun
    }
}
