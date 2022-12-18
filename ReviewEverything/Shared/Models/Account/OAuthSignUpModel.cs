using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Models.Account
{
    public class OAuthSignUpModel
    {
        [Required]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; } = default!;

        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; } = default!;
    }
}
