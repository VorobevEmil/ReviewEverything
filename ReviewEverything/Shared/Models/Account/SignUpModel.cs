using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Models.Account
{
    public class SignUpModel
    {
        [Required]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = default!;

        [Required]
        [Display(Name = "Имя Пользователя")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Имя Пользователя может содерать только латинские буквы")]
        public string UserName { get; set; } = default!;

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(30, ErrorMessage = "Длина пароля должна составлять не менее 8 символов", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = default!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string RePassword { get; set; } = default!;

    }
}
