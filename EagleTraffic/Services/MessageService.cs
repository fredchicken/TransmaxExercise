using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EagleTraffic.Services
{
    public class MessageService : IMessageService
    {
        private readonly string _url;
        private readonly string _exchange;
        public MessageService(string url)
        {
            _url = url;
            _exchange = "RabbitMQ_topc_logs";
        }

        public void Send<T>(T message, string topic)
        {
            if (_url.IsNullOrEmpty()) { return; }

            var factory = new ConnectionFactory { Uri = new Uri(_url) };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Topic);

            var stringMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var body = Encoding.UTF8.GetBytes(stringMessage);
            channel.BasicPublish(exchange: _exchange,
                                 routingKey: topic,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
