using ConnectToDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflow.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IServiceProvider _service;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            var _conversationOverflowDbContext = _service.GetService(typeof(ConversationOverflowDbContext)) as ConversationOverflowDbContext;
            return _conversationOverflowDbContext.Users;
            /*return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();*/
        }

        /*public IEnumerable<User> GetUsers()
        {
            return _conversationOverflowDbContext.Users;
        }*/
    }
}
