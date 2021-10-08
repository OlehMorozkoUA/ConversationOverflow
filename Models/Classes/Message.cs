using System;
using Models.Interfaces;

namespace Models.Classes
{
    public class Message : IMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public int AttachmentId { get; set; }

        public Attachment Attachment { get; set; }
    }
}
