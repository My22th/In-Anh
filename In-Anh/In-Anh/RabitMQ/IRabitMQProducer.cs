using In_Anh.Controllers;
using RabbitMQ.Client;

namespace In_Anh.RabitMQ
{
    public interface IRabitMQProducer 
    {
        public void SendProductMessage(IConnection connection, IModel channel, byte[] body);
    }
}
