namespace Elearn_temp.Models
{
    public abstract class BaseEntity
    {

        public int Id { get; set; }
        public bool SoftDelete { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.Now;



    }
}
