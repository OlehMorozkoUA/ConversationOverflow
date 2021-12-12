using System;

namespace Models.Interfaces
{
    interface IAttachment : IIdentity<int>
    {
        public DateTime CreationTime { get; set; }
        public string Path { get; set; }
    }
}
