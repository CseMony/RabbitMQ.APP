using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rabbit.Consumer
{
    public class Program
    {
        static void Main(string[] args)
        {


            Task.Run(() => Test());
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }


        public static void Test()
        { 
            var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

            channel.QueueDeclare("topic-demo-queue", durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

            channel.QueueBind(queue: "topic-demo-queue",
                                  exchange: "topic_logs",
                                  routingKey: "account.*");
            Console.WriteLine(" [*] Waiting for messages.");

           // channel.BasicQos(0,4,false);
            var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();


        var message = Encoding.UTF8.GetString(body);
        User user = JsonSerializer.Deserialize<User>(message);
        var routingKey = ea.RoutingKey;
        var DelivaryTag = ea.DeliveryTag;


        Console.WriteLine(" [x] {0}:{1} ", message, routingKey);
                Console.WriteLine("{0}", DelivaryTag);
                Thread.Sleep(4000);
                //Console.WriteLine("process done");
                channel.BasicAck(ea.DeliveryTag, false);

            };

    channel.BasicConsume(queue: "topic-demo-queue",
                                 autoAck: false,
                                 consumer: consumer);

            //Pull API
            //while(true){
            //    bool noAck = true;
            //    BasicGetResult result = channel.BasicGet("topic-demo-queue2", noAck);

            //    if (result == null)
            //    {
            //        // No message available at this time.
            //    }
            //    else
            //    {
            //        IBasicProperties props = result.BasicProperties;
            //        ReadOnlyMemory<byte> body = result.Body;
            //        channel.BasicAck(result.DeliveryTag, true);
            //        var resultTag = result.DeliveryTag;
            //        Console.WriteLine(resultTag);
            //        Console.WriteLine("{0}", body);
            //        var bodym = body.ToArray();


            //        var message = Encoding.UTF8.GetString(bodym);
            //        User user = JsonSerializer.Deserialize<User>(message);


            //Thread.Sleep(4000);
            //        Console.WriteLine(" [x] {0} ", message);

                //}

            //}

        }


        }
    
}
