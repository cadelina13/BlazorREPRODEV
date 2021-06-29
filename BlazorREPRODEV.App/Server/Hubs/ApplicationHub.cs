using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Server.Hubs
{
    public class ApplicationHub : Hub
    {
        public async Task NotifyChat(string chatId)
        {
            await Clients.Others.SendAsync("NotifyChat", chatId);
        }
        public async Task NotifyBoardExamPage(string id)
        {
            await Clients.All.SendAsync("NotifyBoardExamPage", id);
        }
        public async Task NotifyRegisteredStudentPage(string userType)
        {
            await Clients.All.SendAsync("NotifyRegisteredStudentPage", userType);
        }
        public async Task NotifyExamPage()
        {
            await Clients.All.SendAsync("NotifyExamPage");
        }
        public async Task NotifyMyExamPage()
        {
            await Clients.All.SendAsync("NotifyMyExamPage");
        }
        public async Task NotifyToggleDrawer(string guid)
        {
            await Clients.All.SendAsync("NotifyToggleDrawer", guid);
        }
    }
}
