namespace EntityFramework_Slider.Models
{
    public class Adventage:BaseEntity
    {
        public string? Icon { get; set; }
        public string? Description { get; set; }

        public int AboutId { get; set; }

        public About About { get; set; }

    }
}
