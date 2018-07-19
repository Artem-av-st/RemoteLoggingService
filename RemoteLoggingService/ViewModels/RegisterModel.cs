using System.ComponentModel.DataAnnotations;

namespace RemoteLoggingService.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name can not be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email can not be empty")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage ="Invalid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password can not be empty")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The password must be between 5 and 20 characters in length")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password must match")]
        public string ConfirmPassword { get; set; }

        public bool Captcha { get; set; }
    }
}
