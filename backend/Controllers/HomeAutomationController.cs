using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeAutomationController : ControllerBase
    {
        private readonly MqttService _mqttService;

        public HomeAutomationController(MqttService mqttService)
        {
            _mqttService = mqttService;

            Console.WriteLine("HomeAutomationController CONSTRUCTER RUNNED");

            // Subscribe to the message received event
            _mqttService.OnMessageReceived += (topic, payload) =>
            {
                // Handle the message received (update dashboard, etc.)
                Console.WriteLine($"Message received on topic {topic}: {payload}");
            };


        }

        [HttpPost("publish")]
        public async Task<IActionResult> PublishCommand([FromQuery] string topic, [FromQuery] string command)
        {
            await _mqttService.PublishAsync(topic, command);
            return Ok("Command sent successfully.");
        }

    }
}
