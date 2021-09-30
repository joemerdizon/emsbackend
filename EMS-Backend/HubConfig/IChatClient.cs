using Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.HubConfig
{
    public interface IChatClient
    {
        Task RecieveMessage(ChatMessage message);
        Task SendMessageToClient(ChatMessage message);
        Task Notify(string userName); // For notification only
    }
}

