using System.Text;
using CertiWeb.API.Inspections.Domain.Model.Commands;
using CertiWeb.API.Inspections.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CertiWeb.API.Shared.Infrastructure.Messaging;

public class CertificateConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CertificateConsumerService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public CertificateConsumerService(
        IConfiguration configuration,
        ILogger<CertificateConsumerService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        try
        {
            var host = _configuration["RabbitMQ__Host"]
                    ?? _configuration["RabbitMQ:Host"]
                    ?? "rabbitmq";
            var factory = new ConnectionFactory { HostName = host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "inspection.completed",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(
                    "[RabbitMQ] Mensaje recibido en inspection.completed: {Message}",
                    message
                );

                try
                {
                    // BackgroundService itself has no scoped services in its constructor, so a
                    // new DI scope is created per message to resolve the scoped DbContext-backed
                    // command service (AC-03: persist real evidence of async processing).
                    using var scope = _serviceScopeFactory.CreateScope();
                    var commandService = scope.ServiceProvider
                        .GetRequiredService<IProcessedInspectionEventCommandService>();
                    await commandService.Handle(new CreateProcessedInspectionEventCommand(message));

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Deliberately not acking: leaving the delivery unacknowledged lets RabbitMQ
                    // redeliver it (e.g. after a connection reset or consumer restart) instead of
                    // silently losing the event because the DB write failed.
                    _logger.LogError(
                        ex,
                        "[RabbitMQ] Fallo al persistir el evento inspection.completed, no se hace ACK: {Message}",
                        message
                    );
                }
            };

            _channel.BasicConsume(
                queue: "inspection.completed",
                autoAck: false,
                consumer: consumer
            );

            _logger.LogInformation(
                "[RabbitMQ] CertificateConsumerService escuchando en inspection.completed"
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "[RabbitMQ] Consumer no pudo conectar: {Message}",
                ex.Message
            );
        }

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
