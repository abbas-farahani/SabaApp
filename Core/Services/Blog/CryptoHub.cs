//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Core.Domain.Contracts.Repositories.Blog;
//using Core.Domain.Contracts.Services.Crypto;
//using Core.Domain.Entities.Crypto;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.DependencyInjection;

//namespace Core.Services.Blog;

//public class CryptoHub : Hub, ICryptoHub
//{
//    private static readonly ConcurrentDictionary<string, string> _subscribedUsers = new();
//    private readonly IServiceScopeFactory _serviceScopeFactory;
//    private readonly ICrypto15mService _crypto15;

//    public CryptoHub(IServiceScopeFactory serviceScopeFactory, ICrypto15mService crypto15)
//    {
//        _serviceScopeFactory = serviceScopeFactory;
//        _crypto15 = crypto15;
//    }

//    public async Task ShowDateTime(string coin)
//    {
//        var latestData = await _crypto15.GetLastDataByName(coin);
//        await Clients.Caller.SendAsync("testresponse", DateTime.Now.ToString());
//    }

//    public async Task SubscribeToUpdates(string userName)
//    {
//        var connectionId = Context.ConnectionId;
//        _subscribedUsers[connectionId] = userName;
//        await Groups.AddToGroupAsync(connectionId, userName);
//    }

//    public async Task UnsubscribeFromUpdates()
//    {
//        var connectionId = Context.ConnectionId;
//        if (_subscribedUsers.TryRemove(connectionId, out var userName))
//        {
//            await Groups.RemoveFromGroupAsync(connectionId, userName);
//        }
//    }

//    public override async Task OnDisconnectedAsync(Exception exception)
//    {
//        await UnsubscribeFromUpdates();
//        await base.OnDisconnectedAsync(exception);
//    }

//    public static string[] GetActiveUserNames()
//    {
//        return _subscribedUsers.Values.Distinct().ToArray();
//    }

//    public async Task PublishUpdateAsync(string userName, Crypto15 latestData)
//    {
//        await Clients.Group(userName).SendAsync("ReceiveUpdate", userName);
//        await Clients.Caller.SendAsync("ReceiveUpdate", latestData);
//    }

//    public async Task ErrorOccurredAsync(string err)
//    {
//        await Clients.Caller.SendAsync("RecieveError", err);
//    }

//}
