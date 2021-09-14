using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Publisher.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IBus _busService;
        public UserController(IBus busService)
        {
            _busService = busService;
        }
        [HttpPost]
        public async Task<string> CreateUser(User user)
        {
            if (user != null)
            {
                user.Datetime = DateTime.Now;
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
               
                {
                    channel.ConfirmSelect();
                    
                    channel.ExchangeDeclare(exchange: "topic_logs",
                                    type: "topic");
                    //channel.QueueDeclare(queue: "UserQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    //var queueName = channel.QueueDeclare().QueueName;
                    var json = JsonConvert.SerializeObject(user);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "topic_logs",
                                 routingKey: "account.init",
                                 basicProperties: null,
                                 body: body);
                    //channel.BasicPublish(exchange: "", routingKey: "UserQueue", basicProperties: null, body: body);

                    

                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                    return "true";
                }
            }
            return "false";
        }
    }
}
