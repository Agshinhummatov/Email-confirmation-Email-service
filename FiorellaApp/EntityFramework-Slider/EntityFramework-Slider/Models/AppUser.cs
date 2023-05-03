using Microsoft.AspNetCore.Identity;

namespace EntityFramework_Slider.Models
{
    public class AppUser:IdentityUser //identityden miras almalidir
    {
        public string FullName { get; set; }

    }
}
