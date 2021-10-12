using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflow.Dto
{
    public class RegistrateUserDto
    {
        [Required(ErrorMessage = "login is required")]
        [StringLength(10)]
        public string Login { get; set; }
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "first name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "birthday is required")]
        public DateTime Birthday { get; set; }
    }
}
