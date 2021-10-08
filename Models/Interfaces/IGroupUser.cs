namespace Models.Interfaces
{
    interface IGroupUser : IIdentity<int>
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
