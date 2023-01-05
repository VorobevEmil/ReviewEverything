using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Models.Account
{
    public class OAuthSignUpModel
    {
        [Required]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = default!;

        [Required]
        [Display(Name = "Имя Пользователя")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Имя пользователя может содержать Латинский алфавит, а также цифры")]
        public string UserName { get; set; } = default!;
    }
}
