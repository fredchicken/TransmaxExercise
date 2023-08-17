using EagleTraffic;
using EagleTraffic.Services;
using EagleTraffic.Controllers;
using EagleTraffic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Data.Sqlite;

namespace TransmaxExerciseTest
{
    public class UnitTest1
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private const string AMQPUrl = "amqps://dqonzfla:G7Ixgrkr4yybHUg9IXr7ZPZqvx5E9xYT@mouse.rmq5.cloudamqp.com/dqonzfla";
        private readonly SqliteConnection _connection;

        private readonly IDistributedCache _cache;
        private readonly DbContextOptions<EagleContext> _dbOptions;
        private readonly EagleContext _dbContext;
        private readonly IEagleTrafficService _service;
        private readonly IMessageService _messageService;
        private readonly EagleBotsController _controller;
        public UnitTest1()
        {
            //setup dbcontext (Sqlite in memory or Sql server)
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            _dbOptions = new DbContextOptionsBuilder<EagleContext>()
                .UseSqlite(_connection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;
            //_dbOptions = new DbContextOptionsBuilder<EagleContext>()
            //    .UseSqlServer("")
            //    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            //    .Options;
            _dbContext = new EagleContext(_dbOptions);
            _dbContext.Database.EnsureCreated();

            //setup cache (in memory cache rather than redis)
            var cacheOptions = Options.Create(new MemoryDistributedCacheOptions());
            _cache = new MemoryDistributedCache(cacheOptions);

            //create service
            _service = new EagleTrafficService(_dbContext, _cache);
            _messageService = new MessageService(AMQPUrl);
            //data feed
            InitialiseDatabase();

            //create the controller to be tested
            _controller = new EagleBotsController(_service, _messageService);
        }

        private void InitialiseDatabase()
        {
            List<EagleBot> eagleBots = new List<EagleBot> {
                new EagleBot
                {
                    Id = 1,
                    Lattitude = 10,
                    Longitude = 20,
                    DateTimeReported = DateTime.Now,
                    RoadName = "AAA",
                    FlowDirection = false,
                    FlowRate = 77,
                    AvgVehicleSpeed = 76
                },
                new EagleBot
                {
                    Id = 2,
                    Lattitude = 100,
                    Longitude = 200,
                    DateTimeReported = DateTime.Now,
                    RoadName = "BBB",
                    FlowDirection = false,
                    FlowRate = 33,
                    AvgVehicleSpeed = 66
                },
                new EagleBot
                {
                    Id = 3,
                    Lattitude = 299,
                    Longitude = 321,
                    DateTimeReported = DateTime.Now,
                    RoadName = "CCC",
                    FlowDirection = true,
                    FlowRate = 11,
                    AvgVehicleSpeed = 33
                }
            };
            foreach (var bot in eagleBots)
            {
                _service.AddEagleBot(bot);
            }
            _dbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task TestGet()
        {
            var result = await _controller.Get();
            Assert.True(result.Count() == 3);
        }

        [Fact]
        public async Task TestUpdate()
        {
            var eagleBotToBeUpdated = new EagleBot()
            {
                Id = 3,
                Lattitude = 101,
                Longitude = 222,
                DateTimeReported = DateTime.Now,
                RoadName = "ABC",
                FlowDirection = false,
                FlowRate = 77,
                AvgVehicleSpeed = 75
            };

            var allEagleBotsBeforeUpdate = await _service.GetAllEagleBots();
            int countBeforeUpdate = allEagleBotsBeforeUpdate.Count();
            bool isEagleBotExisting = allEagleBotsBeforeUpdate.Any(eb => eb.Id == eagleBotToBeUpdated.Id);

            await _controller.Put(eagleBotToBeUpdated);

            int countAfterUpdate = (await _service.GetAllEagleBots()).Count();

            Assert.True(isEagleBotExisting && countBeforeUpdate == countAfterUpdate);
        }

        [Fact]
        public async Task TestAdd()
        {
            var eagleBotToBeUpdated = new EagleBot()
            {
                Id = 6,
                Lattitude = 101,
                Longitude = 222,
                DateTimeReported = DateTime.Now,
                RoadName = "ABC",
                FlowDirection = false,
                FlowRate = 77,
                AvgVehicleSpeed = 87
            };

            var allEagleBotsBeforeUpdate = await _service.GetAllEagleBots();
            int countBeforeUpdate = allEagleBotsBeforeUpdate.Count();
            bool isEagleBotExisting = allEagleBotsBeforeUpdate.Any(eb => eb.Id == eagleBotToBeUpdated.Id);

            await _controller.Put(eagleBotToBeUpdated);

            int countAfterUpdate = (await _service.GetAllEagleBots()).Count();

            Assert.True(!isEagleBotExisting && countBeforeUpdate + 1 == countAfterUpdate);
        }

    }
}