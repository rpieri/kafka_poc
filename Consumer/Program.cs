using Avro.Customer;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Consumer.Workers;
using Microsoft.Extensions.Options;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<ConsumerConfig>(hostContext.Configuration.GetSection("Kafka"));
        services.Configure<SchemaRegistryConfig>(hostContext.Configuration.GetSection("SchemaRegistry"));

        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();
            return new CachedSchemaRegistryClient(config.Value);
        });
    
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<ConsumerConfig>>();
            var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();
            return new ConsumerBuilder<String, Customer>(config.Value)
                .SetValueDeserializer(new AvroDeserializer<Customer>(schemaRegistry).AsSyncOverAsync())
                .Build();
        });

        services.AddHostedService<CustomerLogWorker>();
    })
    .Build();
    
    await host.RunAsync();