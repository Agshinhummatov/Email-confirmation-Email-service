namespace Elearn_temp.Models
{
    public class Author:BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Course> Course { get; set; }



    }
}
