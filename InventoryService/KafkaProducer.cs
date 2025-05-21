using Confluent.Kafka;

namespace InventoryService
{
    public class KafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        public KafkaProducer(string broker)
        {
            var config = new ProducerConfig { BootstrapServers = broker };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessage(string topic, string message)
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }
}
