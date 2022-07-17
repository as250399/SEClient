using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SEClient.Rabbit
{
    public class TlogFromRabbit
    {
        private static ConnectionFactory connectionFactory = new ConnectionFactory();
        private static IConnection connection = connectionFactory.CreateConnection();


        public static event EventHandler UpdateRabbitMessage;

        //protected static void OnUpdateRabbitMessage(UpdateMessageEventArgs e)
        //{
        //    EventHandler handler = UpdateRabbitMessage;
        //    handler?.Invoke(null, e);
        //}
        public static void GetMessagesFromRabbit()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "admin", Password = "admin", Port = 5672 };
            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            Receive("EsbTlogsQueue", channel);
        }


        public static void Receive(string queue, IModel channel)
        {
            IDictionary<String, Object> args2 = new Dictionary<String, Object>();
            args2.Add("SearchEngine", 256);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(consumer, queue, autoAck: true, arguments: args2);
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var BodyArray = body.Span.ToArray(); ;
            string decodedString = Encoding.UTF8.GetString(BodyArray);
            //OnUpdateRabbitMessage(new UpdateMessageEventArgs() { RabbitMessage = decodedString.Substring(decodedString.IndexOf('<')) });
        }
    }
}