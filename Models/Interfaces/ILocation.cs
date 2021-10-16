using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Interfaces
{
    interface ILocation : IIdentity<int>
    {
        public string Country { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public int Postcode { get; set; }
        public int UserId { get; set; }
    }
}
