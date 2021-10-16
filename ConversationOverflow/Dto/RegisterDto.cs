using System;
using System.ComponentModel.DataAnnotations;

namespace ConversationOverflow.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "login is required")]
        [StringLength(10)]
        public string Login { get; set; }
        [Required(ErrorMessage = "email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "first name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Required(ErrorMessage = "confirmation of password is required")]
        [Compare("Password", ErrorMessage = "Пароль не співпадає")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердити пароль")]
        public string PasswordConfirm { get; set; }
        [Required(ErrorMessage = "birthday is required")]
        public DateTime Birthday { get; set; }
    }
}
