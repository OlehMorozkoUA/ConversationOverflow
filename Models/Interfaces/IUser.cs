using Models.Classes;
using System;

namespace Models.Interfaces
{
    interface IUser : IIdentity<int>
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string ImagePath { get; set; }
        public Location Location { get; set; }
        public Status Status { get; set; }
    }
}
