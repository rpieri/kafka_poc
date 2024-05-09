using System.Net;
using Avro;
using Avro.Customer;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Producer.ViewModel;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IProducer<string, Customer> _producer;
    private readonly ILogger<CustomerController> _logger;
    private const string CustomerTopic = "customer";

    public CustomerController(IProducer<String, Customer> producer, ILogger<CustomerController> logger)
    {
        _producer = producer;
        _logger = logger;

        logger.LogInformation("CustomerController is active");
    }


    [HttpGet]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public String Ping()
    {
        _logger.LogInformation("Pong");
        return "Pong";
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerViewModel), (int)HttpStatusCode.Accepted)]
    public async Task<AcceptedResult> Post(CustomerViewModel customerViewModel)
    {
        _logger.LogInformation("Accpted Customer");
        var customer = new Customer
        {
            age = customerViewModel.Age,
            name = customerViewModel.Name,
            id = customerViewModel.Id.ToString()
        };

        var message = new Message<String, Customer>
        {
            Key = customerViewModel.Name,
            Value = customer
        };

        var result = await _producer.ProduceAsync(CustomerTopic, message);

    _producer.Flush();

        return Accepted("", customerViewModel);
    }

}