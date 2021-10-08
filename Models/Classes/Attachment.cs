using Models.Interfaces;
using System;

namespace Models.Classes
{
    public class Attachment : IAttachment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public DateTime CreationTime { get; set; }
        public string Path { get; set; }
        public string Href { get; set; }
        public int Size { get; set; }
        public bool isSavedOnServer { get; set; }
        public int MessageId { get; set; }
    }
}
