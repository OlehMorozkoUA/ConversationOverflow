using Models.Interfaces;
using System;

namespace Models.Classes
{
    public class Attachment : IAttachment
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string Path { get; set; }
    }
}
