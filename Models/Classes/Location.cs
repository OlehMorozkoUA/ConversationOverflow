using Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace Models.Classes
{
    public class Location : ILocation 
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public int Postcode { get; set; }
        public int UserId { get; set; }

        public Task<Location> Take()
        {
            throw new NotImplementedException();
        }
    }
}
