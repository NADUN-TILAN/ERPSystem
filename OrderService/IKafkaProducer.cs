using Confluent.Kafka;

namespace OrderService
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, string message);
    }

    public class KafkaProducer : IKafkaProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProducer(IConfiguration configuration)
        {
            _config = new ProducerConfig { BootstrapServers = configuration["Kafka:BootstrapServers"] };
        }

        public async Task ProduceAsync(string topic, string message)
        {
            using var producer = new ProducerBuilder<Null, string>(_config).Build();
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }
}