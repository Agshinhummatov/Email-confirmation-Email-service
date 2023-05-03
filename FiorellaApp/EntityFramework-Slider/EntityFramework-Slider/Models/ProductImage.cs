namespace EntityFramework_Slider.Models
{
    public class ProductImage:BaseEntity
    {
        public string Image { get; set; }
        public bool IsMain { get; set; } = false;
        public int ProductId { get; set; }

        //product tipinden product yaziriq. relation qoymaq uchun prodictid ni qoyduq, foreign key eledik, sonra
        //productImageden yola chixanda bilekki bu product image hansi producta aiddi. yeni wekilden yola chixdiqda productun ichindeki her weye chata bileceyik
        public Product Product { get; set; }
    }
}
