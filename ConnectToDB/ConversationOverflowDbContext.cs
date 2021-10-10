using Microsoft.EntityFrameworkCore;
using Models.Classes;

namespace ConnectToDB
{
    public class ConversationOverflowDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }

        public ConversationOverflowDbContext(DbContextOptions<ConversationOverflowDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
