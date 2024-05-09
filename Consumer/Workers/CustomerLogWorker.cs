using Avro.Customer;
using Confluent.Kafka;

namespace Consumer.Workers;

public class CustomerLogWorker : BackgroundService
{
    private readonly IConsumer<string, Customer> _consumer;
    private readonly ILogger<CustomerLogWorker> _logger;
    private const String CustomerTopic = "customer";

    public CustomerLogWorker(IConsumer<String, Customer> consumer, ILogger<CustomerLogWorker> logger)
    {
        _consumer = consumer;
        _logger = logger;
        
        _logger.LogInformation("Customer Worker is Active");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(CustomerTopic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);
            await HandleMessage(result.Message.Value, stoppingToken);
        }
        
        _consumer.Close();
    }

    
    
    private async Task HandleMessage(Customer messageValue, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Novo Customer ID: { messageValue.id} Name: {messageValue.name} Age: {messageValue.age}");
    }
}