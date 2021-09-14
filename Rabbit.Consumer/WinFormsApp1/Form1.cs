using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private ConnectionFactory factory = null;
        private IConnection connection = null;
        private IModel channel = null;
        public Form1()
        {
            InitializeComponent();
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            Test();
        }
        public void Test()
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //channel.ExchangeDeclare(exchange: "direct_logs",
            //                    type: "direct");
            //var queueName = channel.QueueDeclare().QueueName;


            //foreach (var key in routing_keys)
            //{
            //    channel.QueueBind(queue: queueName,
            //                      exchange: "direct_logs",
            //                      routingKey: key);
            //}
            //channel.QueueBind(queue: "UserQueue",
            //                      exchange: "",
            //                      routingKey: "UserQueue");
            Console.WriteLine(" [*] Waiting for messages.");


            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Task.Run(() =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    User user = JsonSerializer.Deserialize<User>(message);
                    var routingKey = ea.RoutingKey;
                    Thread.Sleep(10000);
                    Debug.WriteLine(" [x] {0}:{1} ", message, routingKey);
                });
            };
            channel.BasicConsume(queue: "UserQueue",
                                 autoAck: true,
                                 consumer: consumer);
        }


    }
    public class User
    {
        public int id { get; set; }
        public int index { get; set; }
        public DateTime Datetime { get; set; }
    }
}
