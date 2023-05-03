namespace Elearn_temp.Models
{
    public class CourseImage:BaseEntity
    {
        public string Image { get; set; }
        public bool IsMain { get; set; } = false;
        public int CoruseId { get; set; }
        public Course Coruse { get; set; }
    }
}
