using System.Collections.Generic;

namespace Models.Interfaces
{
    interface IUserMessage : IIdentity<int>
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int? AttachmentId { get; set; }
    }
}
