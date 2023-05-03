namespace EntityFramework_Slider.Models
{
    public class Setting: BaseEntity  //setting classini static datalari(logo,phone ve s) saxlamag uchun yaradiriq. key value ile iwleyir
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
