using Blazored.LocalStorage;
using BlazorREPRODEV.App.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorREPRODEV
{
    public class Core
    {
        private static readonly Core _instance = new Core();
        private static HubConnection hubConnection;
        public UserAccount LoggedUser = new UserAccount();

        public async Task<HubConnection> GetHub(string host)
        {
            hubConnection = new HubConnectionBuilder().WithUrl($"{host}hubs/applicationHub").WithAutomaticReconnect().Build();
            if (hubConnection.State != HubConnectionState.Connected)
                await hubConnection.StartAsync();
            return hubConnection;
        }


        public static Core GetInstance()
        {
            return _instance;
        }
        public async Task SetUser(ILocalStorageService localStorage, UserAccount user)
        {
            await localStorage.SetItemAsync("user", user);
            LoggedUser = user;
        }
        public async Task<UserAccount> GetUser(ILocalStorageService localStorage)
        {
            var user = await localStorage.GetItemAsync<UserAccount>("user");
            return user;
        }
        public async Task RemoveUser(ILocalStorageService localStorage)
        {
            await localStorage.RemoveItemAsync("user");
        }
    }
}
