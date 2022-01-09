using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UdemyRabbitMQ.WaterMarkWeb.Services;

namespace UdemyRabbitMQ.WaterMarkWeb.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMqClientService _rabbitMqClientService;
        private readonly ILogger<RabbitMqClientService> _logger;
        private IModel _chanell; 

        public ImageWatermarkProcessBackgroundService(RabbitMqClientService rabbitMqClientService, ILogger<RabbitMqClientService> logger)
        {
            _rabbitMqClientService = rabbitMqClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _chanell = _rabbitMqClientService.Connect();
            _chanell.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumer = new AsyncEventingBasicConsumer(_chanell);
            _chanell.BasicConsume(RabbitMqClientService.QueueName, false, consumer);
            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private  Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", productImageCreatedEvent.ImageName);
                string siteName = "www.nuh.com";
                using var img = Image.FromFile(path);
                using var graphic = Graphics.FromImage(img);
                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);
                var textSize = graphic.MeasureString(siteName, font);
                var color = Color.FromArgb(128, 255, 255, 255);
                var brush = new SolidBrush(color);
                var postion = new Point(img.Width - ((int)textSize.Width + 30), img.Height - (int)textSize.Height + 30);
                graphic.DrawString(siteName, font, brush, postion);
                img.Save("wwwroot/Images/watermarks/" + productImageCreatedEvent.ImageName);
                img.Dispose();
                graphic.Dispose();
                _chanell.BasicAck(@event.DeliveryTag,false);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message.ToString());
            }

               return  Task.CompletedTask;

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
