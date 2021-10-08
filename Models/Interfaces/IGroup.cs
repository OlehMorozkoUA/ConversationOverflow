using System;

namespace Models.Interfaces
{
    interface IGroup : IIdentity<int>
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
