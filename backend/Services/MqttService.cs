using backend.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
//using MQTTnet.Client.Options;
using System.Text;
using System.Threading.Tasks;

public class MqttService
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _mqttOptions;

    private ILogger<MqttService> _logger;

    private readonly IHubContext<SignalRHub> _hubContext;

    public event Action<string, string>? OnMessageReceived;//The event handler will receive the topic and payload whenever a message is received.

    public MqttService(ILogger<MqttService> logger, IHubContext<SignalRHub> hubContext)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        _logger = logger;
        _hubContext = hubContext; // Inject SignalR Hub

        _mqttOptions = new MqttClientOptionsBuilder()
            .WithClientId("ASP.NETCore_Backend")
            .WithTcpServer("192.168.8.169", 1883)
            .WithCleanSession()
            .Build();

        _logger.LogInformation("MqttService contructer is run");

    }

    public async Task ConnectAsync()
    {
        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected successfully with MQTT Broker.");

            // Subscribe to topics once connected
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter("home/room1/temperature/read").Build());
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter("home/room1/light1/status").Build());

            Console.WriteLine("Subscribed to 'home/room1/temperature/read' topic.");
            Console.WriteLine("Subscribed to 'home/room1/light1/status' topic.");
        };

        _mqttClient.DisconnectedAsync += e =>
        {
            Console.WriteLine("Disconnected from MQTT broker.");
            return Task.CompletedTask;
        };

        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            _logger.LogInformation($"Message received on topic {topic}: {payload}");

            if (topic == "home/room1/temperature/read")
            {
                // Broadcast temperature data using SignalR
                _logger.LogInformation($"Sending {payload} to SignalR");
    
                await _hubContext.Clients.All.SendAsync("ReceiveTemperature", payload);
            }

        };

        await _mqttClient.ConnectAsync(_mqttOptions);
        _logger.LogInformation("MQTT client connected.");
    }

    public async Task PublishAsync(string topic, string payload)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        if (_mqttClient.IsConnected)
        {
            await _mqttClient.PublishAsync(mqttMessage);
            _logger.LogInformation($"Published message to {topic}: {payload}");
        }
        else
        {
            _logger.LogError("MQTT client is not connected. Failed to publish message.");
        }
    }


    public async Task SubscribeAsync(string topic)
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build());
            _logger.LogInformation($"Subscribed to topic {topic}");
        }
        else
        {
            _logger.LogError("MQTT client is not connected. Failed to subscribe.");
        }
    }


}
