using Microsoft.AspNetCore.Identity;

namespace Elearn_temp.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
    }
}
