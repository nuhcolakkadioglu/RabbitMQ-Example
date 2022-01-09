using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {

            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://thziwrmr:sleeDqUl2lx_aIctsXttQFL3u5nXNzAe@jaguar.rmq.cloudamqp.com/thziwrmr");
            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var randomQueuName = channel.QueueDeclare().QueueName;

            channel.QueueBind(randomQueuName, "logs-fanout","",null);
            channel.BasicQos(0, 1,false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(randomQueuName, false, consumer);
            Console.WriteLine("Loglar dinleniyor");
            consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen Mesaş " + message);
                channel.BasicAck(e.DeliveryTag,false);
            };

            Console.ReadLine();
        }

        
    }
}
