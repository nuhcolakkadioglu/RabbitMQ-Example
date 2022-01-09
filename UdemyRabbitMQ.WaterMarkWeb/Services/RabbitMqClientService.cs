using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace UdemyRabbitMQ.WaterMarkWeb.Services
{
    public class RabbitMqClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private  IConnection _connection;
        private  IModel _chanell;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMqClientService> _logger;
        public RabbitMqClientService(ConnectionFactory connectionFactory,ILogger<RabbitMqClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_chanell is { IsOpen: true })
                return _chanell;
            _chanell= _connection.CreateModel();
            _chanell.ExchangeDeclare(ExchangeName, "direct", true,false);
            _chanell.QueueDeclare(QueueName, true, false,false,null);
            _chanell.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWaterMark);
            _logger.LogInformation("RabbitMQ ile bağlantı sağlandı");
            return _chanell;
        }

        public void Dispose()
        {
            _chanell?.Close();
            _chanell?.Dispose();
            _connection = default;

            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile bağlantı koptu");
        }
    }
}
