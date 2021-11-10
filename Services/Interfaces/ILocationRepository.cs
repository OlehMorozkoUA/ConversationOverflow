using Models.Classes;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location> GetLocationByUserIdAsync(int userId);
        Task AddLocationAsync(int userId, Location location);
        Task UpdateLocationAsync(int userId, Location location);
    }
}
