using Microsoft.AspNetCore.SignalR;

namespace backend.Services
{
    public class SignalRHub : Hub
    {
        public async Task SendTemperature(string temperature)
        {
            await Clients.All.SendAsync("ReceiveTemperatureUpdate", temperature);
        }

        public async Task SendLightStatus(string status)
        {
            await Clients.All.SendAsync("ReceiveLightStatus", status);
        }
    }
}
