using System;
using System.Collections.Generic;
using Models.Interfaces;

namespace Models.Classes
{
    public class Group : IGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public int? AdminId { get; set; }
        public bool IsPrivate { get; set; }
        public string ImagePath { get; set; }
    }
}
