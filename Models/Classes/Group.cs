using System;
using Models.Interfaces;

namespace Models.Classes
{
    public class Group : IGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
