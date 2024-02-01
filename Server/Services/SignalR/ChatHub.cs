using BlazorTodo.Shared;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR;

namespace BlazorTodo.Server.Services.SignalR
{
    public class ChatHub : Hub
    {
        // Youtube 참고
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);  
        //}

        // hub with MessageModel obj
        public async Task SendMessageWithModel(MessageModel massageModel)
        {
            await Clients.All.SendAsync("ReceiveMessageWithModel", massageModel);
        }
    }
}
