using System.Collections.Generic;

namespace Models.Interfaces
{
    interface IUserMessage : IIdentity<int>
    {
        public int MessageId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserOrGroupId { get; set; }
        public bool IsToUser { get; set; }
        public int AttachmentId { get; set; }
    }
}
