namespace EntityFramework_Slider.Models
{
    public class About: BaseEntity
    {
        public string? Header { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public ICollection<Adventage> Adventages { get; set; }

    }
}
