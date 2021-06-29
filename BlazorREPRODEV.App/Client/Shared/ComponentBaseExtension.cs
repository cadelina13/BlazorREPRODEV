using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace BlazorREPRODEV.App.Client.Shared
{
    public class ComponentBaseExtension : ComponentBase
    {
        [Inject]
        public NavigationManager nav { get; set; }
        [Inject]
        public HttpClient http { get; set; }
        [Inject]
        public ISnackbar snackbar { get; set; }
        [Inject]
        public IDialogService dialog { get; set; }

        public HubConnection hubConnection;

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder().WithUrl($"{nav.BaseUri}hubs/applicationHub").WithAutomaticReconnect().Build();
        }
    }
}
