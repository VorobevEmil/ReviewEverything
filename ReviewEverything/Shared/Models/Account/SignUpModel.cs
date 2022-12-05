using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Models.Account
{
    public class SignUpModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string Username { get; set; } = default!;

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [StringLength(30, ErrorMessage = "Длина пароля должна составлять не менее 1 символов", MinimumLength = 1)]
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
