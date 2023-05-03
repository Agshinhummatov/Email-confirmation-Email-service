namespace Elearn_temp.Models
{
    public class Course:BaseEntity
    {
        public string Name { get; set; }
        public int Price { get; set; }
      
        
        public string Description { get; set; }

        public int CountSale { get; set; }

        public ICollection<CourseImage> CourseImages { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }

    }
}
