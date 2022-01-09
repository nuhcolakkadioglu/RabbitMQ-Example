using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            //rabbitmq yönetim https://api.cloudamqp.com/console/53c5705f-ce59-4fa1-8cd4-690b2c0f72da/details
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://thziwrmr:sleeDqUl2lx_aIctsXttQFL3u5nXNzAe@jaguar.rmq.cloudamqp.com/thziwrmr");
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            // channel.QueueDeclare("hello-queue", true, false, false);
            channel.ExchangeDeclare("logs-fanout",durable:true,type:ExchangeType.Fanout);
            Enumerable.Range(1, 50).ToList().ForEach(m =>
            {
                string message = "log "+m.ToString();
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", "", null, messageBody);
                Console.WriteLine("Mesaj Gönderimiştir. "+ message);
            });

          
            Console.ReadLine();
        }
    }
}
