using Models.Interfaces;

namespace Models.Classes
{
    public class GroupUser : IGroupUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
