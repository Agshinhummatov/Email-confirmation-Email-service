using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        
        //Burada Icollection yaziriq ProductImage tipinden Images yaziriq, yeni sen eger productdan yola chixsan(meselen product detaile e gedirik),onun imagelerini icherisinde getir.
        // yeni bu productun imageleri hansilar olacaq.
        public ICollection<ProductImage> Images { get; set; }
        //one olan terefe ICollection tipinden qeyd edirik.ona goreki productun ichinde productimagelere chata bilmek uchun.
        //product imageye include vasitesile chatib onun ichindeki imageleri gotururuk.
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
