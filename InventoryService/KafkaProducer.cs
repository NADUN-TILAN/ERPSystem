using Confluent.Kafka;

namespace InventoryService
{
    public class KafkaProducer
    {
        public async Task SendMessageAsync(string topic, string message)
        {
            var config = new ProducerConfig { BootstrapServers = "kafka:9092" };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }

}
