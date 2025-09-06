using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Utilities.HttpContextHelper;
using Utilities.StaticServiceCaller;
using Utilities.User;

namespace MVC.Areas.Dashboard.Hubs;
public interface ICryptoHub
{
    //Task PublishUpdateAsync(string userName, Crypto15 latestData);
    //Task<string> GetActiveUserNames();
}

public static class MyServiceProvider
{
    public static IServiceProvider ServiceProvider { get; set; }
}

public class CryptoHub : Hub, ICryptoHub
{
    #region Construction
    private static ConcurrentDictionary<string, string> _usersConectionId = new(); // key:userId , value:connectionId
    private static ConcurrentDictionary<string, string> _usersCoin = new(); // key:userId , value:coinName
    private static ConcurrentDictionary<string, HttpContext> _connections = new ConcurrentDictionary<string, HttpContext>();
    private static string _userId;
    //private static readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _memoryCache;

    public CryptoHub(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    #endregion

    #region Methods
    public override async Task OnConnectedAsync()
    {
        Context.Items["ConnectionId"] = Context.ConnectionId;
        _userId = Context.User.GetUserId();
        var httpContext = Context.GetHttpContext();
        if (httpContext != null)
            _connections.TryAdd(_userId, httpContext);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Context.Items.Remove("ConnectionId");
        _connections.TryRemove(Context.User.GetUserId(), out _);
        await UnsubscribeFromUpdates();
        await base.OnDisconnectedAsync(exception);
    }

    public void SetConnection()
    {
        var userId = Context.User.GetUserId();
        if (_usersConectionId.TryGetValue(userId, out string oldConnId))
            _usersConectionId.TryUpdate(userId, Context.ConnectionId, oldConnId);
        else
            _usersConectionId[userId] = Context.ConnectionId;
    }

    public void SetCoin(string coin)
    {
        var userId = Context.User.GetUserId();
        if (_usersCoin.TryGetValue(userId, out string oldCoin))
        {
            _usersCoin.TryUpdate(userId, coin, oldCoin);
            Clients.Caller.SendAsync("ReceiveUpdate", $"{coin} coin is set");
        }
        else
        {
            _usersCoin[userId] = coin;
            Clients.Caller.SendAsync("ReceiveUpdate", $"{coin} coin is not set");
        }
    }

    public static bool SetCoin(string userId, string coin)
    {
        bool result = false;
        if (_usersCoin.TryGetValue(userId, out string oldCoin))
            result = _usersCoin.TryUpdate(userId, coin, oldCoin);
        else
            result = _usersCoin.TryAdd(userId, coin);

        return result;
    }


    public void ErrorOccurred(string err)
    {
        Clients.Caller.SendAsync("RecieveError", err);
    }

    private async Task UnsubscribeFromUpdates()
    {
        var connectionId = Context.ConnectionId;
        var userId = _usersConectionId.FirstOrDefault(x => x.Value == connectionId).Key;
        _usersConectionId.TryRemove(userId, out _);
        _usersCoin.TryRemove(userId, out _);
    }

    public static UserCoin? GetUserCoin()
    {
        try
        {
            var context = StaticServiceCaller.GetService<IMemoryCache>();
            if (!string.IsNullOrEmpty(_userId))
            {
                var httpContext = GetHttpContext(_userId);
                if (httpContext == null) return null;

                string connId = httpContext.Connection.Id;
                if (httpContext != null)
                {
                    // دسترسی به ویژگی ConnectionId از طریق Items
                    if (httpContext.Items.TryGetValue("ConnectionId", out var connectionId) || !string.IsNullOrEmpty(connId))
                    {
                        return new UserCoin
                        {
                            UserId = _userId,
                            ConnectionId = connId ?? connectionId.ToString(),
                            Coin = _usersCoin.Count() > 0 ? _usersCoin[_userId] : "eth"
                        };
                    }
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static HttpContext GetHttpContext(string userId)
    {
        if (_connections.TryGetValue(userId, out var httpContext))
        {
            return httpContext;
        }
        return null;
    }
    #endregion
}



public class UserCoin
{
    public string UserId { get; set; }
    public string Coin { get; set; }
    public string? ConnectionId { get; set; }
}