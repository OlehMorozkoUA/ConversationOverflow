using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using Models.Classes;
using Services.Interfaces;
using System.Threading.Tasks;

namespace Services.Classes.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ConversationOverflowDbContext _conversationOverflowDbContext;
        public LocationRepository(ConversationOverflowDbContext conversationOverflowDbContext)
        {
            _conversationOverflowDbContext = conversationOverflowDbContext;
        }
        public async Task<Location> GetLocationByUserIdAsync(int userId)
            => await _conversationOverflowDbContext.Locations.FirstOrDefaultAsync(location => location.UserId == userId);

        public async Task AddLocationAsync(int userId, Location location)
        {
            Location loc = await GetLocationByUserIdAsync(userId);

            if (loc == null)
            {
                location.UserId = userId;
                await _conversationOverflowDbContext.Locations.AddAsync(location);
                await _conversationOverflowDbContext.SaveChangesAsync();
            }
        }
        
        public async Task UpdateLocationAsync(int userId, Location location)
        {
            Location loc = await GetLocationByUserIdAsync(userId);

            if (loc != null)
            {
                loc.Country = location.Country;
                loc.Region = location.Region;
                loc.Address = location.Address;
                loc.Postcode = location.Postcode;
                await _conversationOverflowDbContext.SaveChangesAsync();
            }
        }
    }
}
