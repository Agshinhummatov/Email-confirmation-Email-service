using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.ViewModels.Account
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress,ErrorMessage = "E-mail is not vaild")] // bu  neynir yoxlayirki emaildir @ var ya yox eger yoxdursa bu mesaji yazir
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password),Compare(nameof(Password))]  // bu neynir yoxlayirki yuxardaki pawword ile 2 ci password eyndir yeni Compare yoxlamaq demekdir
        public string ConfirmPassword { get; set; }

    }
}
