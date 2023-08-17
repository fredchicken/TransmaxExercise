using EagleTraffic.Models;

namespace EagleTraffic.Services
{
    public interface IEagleTrafficService
    {
        public Task<IEnumerable<EagleBot>> GetAllEagleBots();
        public Task AddEagleBot(EagleBot bot);
        public Task UpdateEagleBot(EagleBot bot);

        public bool IsEagleBotExisting(int id);
    }
}
