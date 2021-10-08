using System;

namespace Models.Interfaces
{
    interface IMessage : IIdentity<int>
    {
        public string Text { get; set; }
        public DateTime CreationTime { get; set; }
        public int AttachmentId { get; set; }
    }
}
