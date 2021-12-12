using System;
using Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Classes
{
    public class User : IdentityUser<int>, IUser 
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string ImagePath { get; set; }
        public Status Status { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
