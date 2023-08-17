using Microsoft.Extensions.Caching.Distributed;
using EagleTraffic.Models;

namespace EagleTraffic.Services
{
    public class EagleTrafficService: IEagleTrafficService
    {
        private readonly EagleContext _dbContext;
        private readonly Cache _cache;
        private const string EagleBotsCacheKey = "EagleBots";
        public EagleTrafficService(EagleContext dbContext, IDistributedCache cacheProvider)
        {
            _dbContext = dbContext;
            _cache = new Cache(cacheProvider);
        }
        public async Task<IEnumerable<EagleBot>> GetAllEagleBots()
        {
            IEnumerable<EagleBot> eagleBots;

            eagleBots = await _cache.Get<IEnumerable<EagleBot>>(EagleBotsCacheKey);

            if (eagleBots == null)
            {
                eagleBots = _dbContext.EagleBots.OrderBy(eb => eb.Id);
                await _cache.Set(EagleBotsCacheKey, eagleBots);
            }

            return eagleBots;
        }

        public async Task AddEagleBot(EagleBot bot)
        {
            if (!_dbContext.EagleBots.Any(eb => eb.Id == bot.Id))
            {
                _dbContext.EagleBots.Add(bot);
                _dbContext.SaveChanges();

                var eagleBots = _dbContext.EagleBots.OrderBy(eb => eb.Id);
                await _cache.Set(EagleBotsCacheKey, eagleBots);
            }
        }

        public async Task UpdateEagleBot(EagleBot bot)
        {
            _dbContext.EagleBots.Update(bot);
            _dbContext.SaveChanges();

            var eagleBots = _dbContext.EagleBots.OrderBy(eb => eb.Id);
            await _cache.Set(EagleBotsCacheKey, eagleBots);
        }

        public bool IsEagleBotExisting(int id)
        {
            return _dbContext.EagleBots.Any(eb => eb.Id == id);                
        }
    }
}
