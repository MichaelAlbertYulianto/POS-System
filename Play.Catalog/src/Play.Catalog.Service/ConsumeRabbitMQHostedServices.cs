using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Play.Catalog.Service
{
    public class ConsumeRabbitMQHostedServices : BackgroundService
    {
        private readonly ILogger<ConsumeRabbitMQHostedServices> _logger;
        private IConnection _connection;
        private IChannel _channel; // Changed from RabbitMQ.Client.IModel to just IModel
        private readonly Task _initializazionTask;
        

        public ConsumeRabbitMQHostedServices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedServices>();
            _initializazionTask = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                await InitRabbitMQ();
                _logger.LogInformation("RabbitMQ connection initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initializing RabbitMQ connection");
                // throw new Exception(ex.Message);
            }
        }

        private async Task InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: "task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            // Initalize RabbitMQ connection and setup consumer here
            _logger.LogInformation("RabbitMQ initialized");
        }

        private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs e)
        {
            _logger.LogInformation("RabbitMQ connection shutdown : {Reason}", e.ReplyText);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _initializazionTask;
            if(_channel == null){
                _logger.LogError("RabbitMQ channel is null");
                return;
            }
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Received message: {message}");

                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            consumer.RegisteredAsync += async (model, ea) =>
            {
                _logger.LogInformation("Consumer registered: {CostumerTag}", ea.ConsumerTags);
            };
            consumer.UnregisteredAsync += async (model, ea) =>
            {
                _logger.LogInformation("Consumer unregistered: {CostumerTag}", ea.ConsumerTags);
            };

            await _channel.BasicConsumeAsync("task_queue", autoAck: false, consumer: consumer);
            _logger.LogInformation("Cosumer started. Waiting for messages. . .");
        }

        public override async void Dispose()
        {
            _logger.LogInformation("RabbitMQ connection disposing...");
            try
            {
                if(_channel != null){
                    await _channel.CloseAsync();
                }
                if(_connection != null){
                    await _connection.CloseAsync();
                }
                _logger.LogInformation("RabbitMQ connection disposed");
            }
            catch (Exception ex){
                _logger.LogError(ex, "Failed to dispose RabbitMQ connection");
            }
            finally {
                base.Dispose();
            }
            base.Dispose();
        }
    }
}