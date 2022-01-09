using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UdemyRabbitMQ.WaterMarkWeb.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMqClientService _rabbitMqClientService;

        public RabbitMQPublisher(RabbitMqClientService rabbitMqClientService)
        {
            _rabbitMqClientService = rabbitMqClientService;
        }
        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var chanell = _rabbitMqClientService.Connect();
            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var properties = chanell.CreateBasicProperties();
            properties.Persistent = true;
            chanell.BasicPublish(RabbitMqClientService.ExchangeName, RabbitMqClientService.RoutingWaterMark, properties,bodyByte);

        }
    }
}
