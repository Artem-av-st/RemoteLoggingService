using System.ComponentModel.DataAnnotations;

namespace RemoteLoggingService.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email can not be empty")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password can not be empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Captcha { get; set; }
    }
}
