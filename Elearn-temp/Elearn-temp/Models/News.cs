namespace Elearn_temp.Models
{
    public class News : BaseEntity
    {

        public string? Title { get; set; }

        public string? Author { get; set; }

        public DateTime PublishDate { get; set; }

        public string Image { get; set; }

    }
}
