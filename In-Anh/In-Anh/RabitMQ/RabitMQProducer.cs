using Lucene.Net.Support;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace In_Anh.RabitMQ
{
    public class RabitMQProducer : IRabitMQProducer 
    {
        public void SendProductMessage(IConnection connection, IModel channel, byte[] body)
        {

            try
            { //Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
                
                //Here we create channel with session and model
                 
                //channel.ConfirmSelect();
                var props = new HashMap<string, object>();
                //channel.QueueDeclare("images", exclusive: false);
                channel.QueueDeclareNoWait(queue: "image",
                      durable: true,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);

                //Serialize the message
                
                //put the data on to the product queue
                channel.BasicPublish(exchange: string.Empty,
                     routingKey: "image",
                     basicProperties: null,
                     body: body);

                //channel.BasicPublish(exchange: "", routingKey: "image", body: body);
                //channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(2));
               
            }
            catch (Exception e)
            {
                var a = e.Message;
                throw;
            }

            
            
            //declare the queue after mentioning name and a few property related to that

        }
        public void setTimeout(Action TheAction, int Timeout)
        {
            Thread t = new Thread(
                () =>
                {
                    Thread.Sleep(Timeout);
                    TheAction.Invoke();
                }
            );
            t.Start();
        }
    }
}
