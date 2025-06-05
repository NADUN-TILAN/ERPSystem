using Confluent.Kafka;

namespace UserService
{
    public class KafkaConsumer
    {
        public void Consume(string topic, string broker)
        {
            var kafkaConsumer = new KafkaConsumer();
            Task.Run(() => kafkaConsumer.Consume("user-topic", "localhost:9092"));
        }
    }
}
