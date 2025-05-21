using Confluent.Kafka;

namespace UserService
{
    public class KafkaConsumer
    {
        public void Consume(string topic, string broker)
        {
            var config = new ConsumerConfig
            {
                GroupId = "erp-group",
                BootstrapServers = broker,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            while (true)
            {
                var result = consumer.Consume(CancellationToken.None);
                Console.WriteLine($"Received: {result.Message.Value}");
            }
        }
    }
}
