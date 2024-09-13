using Microsoft.AspNetCore.SignalR;

namespace API.Hubs {
    public class CanvasHub : Hub {
        public async Task LineUpdated(object line) {
            await Clients.Others.SendAsync("OnLineUpdated", line);
        }
    }
}
