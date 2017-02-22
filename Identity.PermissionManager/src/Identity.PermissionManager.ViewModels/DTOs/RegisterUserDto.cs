using System.ComponentModel.DataAnnotations;

namespace Identity.PermissionManager.ViewModels.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Podaj login")]
        [Display(Name = "Login")]
        [StringLength(100, ErrorMessage = " {0} nie może przekraczac {1} znaków")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Podaj email")]
        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = " {0} nie może przekraczac {1} znaków")]
        public string Email { get; set; }

        [Display(Name = "Imię")]
        [StringLength(50, ErrorMessage = " {0} nie może przekraczac {1} znaków")]
        [Required(ErrorMessage = "Podaj imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required(ErrorMessage = "Podaj nazwisko")]
        [StringLength(50, ErrorMessage = " {0} nie może przekraczac {1} znaków")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Podaj hasło")]
        [Display(Name = "Hasło")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = " {0} musi mieć przynajmniej {2} znaków", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potwierdź hasło")]
        [Display(Name = "Potwierdź hasło")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = " {0} musi mieć przynajmniej {2} znaków", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
        
    }
}
