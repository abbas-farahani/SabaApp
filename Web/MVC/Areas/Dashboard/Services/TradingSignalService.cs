using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using MVC.Areas.Dashboard.Hubs;
using MVC.Areas.Dashboard.Models;
using Utilities.CryptoCurrency;
using Utilities.StaticServiceCaller;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MVC.Areas.Dashboard.Services;

//#region Interface
//public interface ITradingSignalService
//{
//    Task StartAsync(CancellationToken cancellationToken);
//    Task StopAsync(CancellationToken cancellationToken);
//    void UpdateUserCoin(string userId, string coin);
//}
//#endregion

//#region Class
//public class TradingSignalService : IHostedService, IDisposable
//{
//    #region Construction
//    private readonly IServiceScopeFactory _serviceScopeFactory;
//    private readonly IHubContext<CryptoHub> _hubContext;
//    private Timer _timer;
//    private Timer _getLlivePriceTimer;
//    private readonly ConcurrentDictionary<string, string> _userCoins = new ConcurrentDictionary<string, string>();
//    private readonly IMemoryCache _memoryCache;
//    private UserCoin _userCoin;

//    public TradingSignalService(IServiceScopeFactory serviceScopeFactory, IHubContext<CryptoHub> hubContext, IMemoryCache memoryCache)
//    {
//        //_userCoin = CryptoHub.GetUserCoin();
//        _timer = new Timer(UpdateCryptoData, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
//        _getLlivePriceTimer = new Timer(GetLivePrice, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
//        _serviceScopeFactory = serviceScopeFactory;
//        _hubContext = hubContext;
//        _memoryCache = memoryCache;
//    }
//    #endregion

//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        return Task.CompletedTask;
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        _timer?.Change(Timeout.Infinite, 0);
//        _getLlivePriceTimer?.Change(Timeout.Infinite, 0);
//        return Task.CompletedTask;
//    }

//    public void UpdateUserCoin(string userId, string coin)
//    {
//        if (_userCoins.TryGetValue(userId, out string oldCoin))
//            _userCoins.TryUpdate(userId, coin, oldCoin);
//        else
//            _userCoins.TryAdd(userId, coin);
//    }

//    private async void UpdateCryptoData(object state)
//    {
//        UserCoin userCoin = CryptoHub.GetUserCoin();
//        try
//        {
//            if (userCoin != null && !string.IsNullOrEmpty(userCoin.Coin))
//            {
//                using (var scope = _serviceScopeFactory.CreateScope())
//                {
//                    //var CryptoService = scope.ServiceProvider.GetRequiredService<ICryptoAppService>();

//                    ////var coin = CryptoUtil.GetCryptoName(userCoin.Coin);
//                    //var latestData = await CryptoService.GetLastDataByName(userCoin.Coin);
//                    //if (latestData != null)
//                    //    await _hubContext.Clients.User(userCoin.UserId).SendAsync("ReceiveUpdate", latestData);
//                    //else
//                    //    await _hubContext.Clients.User(userCoin.UserId).SendAsync("ReceiveUpdate", $"Error on connect to analyst");
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            await _hubContext.Clients.User(userCoin.UserId).SendAsync("ReceiveError", $"Error on get analysis of {userCoin.Coin}");
//        }
//    }
//    private async void GetLivePrice(object state)
//    {
//        UserCoin userCoin = CryptoHub.GetUserCoin();
//        try
//        {
//            if (userCoin == null || string.IsNullOrEmpty(userCoin.Coin)) return;
//            using (var scope = _serviceScopeFactory.CreateScope())
//            {
//                var Api = "null";// scope.ServiceProvider.GetRequiredService<ICoinLayerApi>();
//                string coinName = userCoin.Coin.ToLower();
//                if (!_memoryCache.TryGetValue($"getLivePrice_{coinName}", out string result))
//                {
//                    //var staticservice = StaticServiceCaller.GetService<IMemoryCache>();
//                    //var staticserviceUser = StaticServiceCaller.HttpContext?.User;
//                    //var staticserviceUser2 = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
//                    result = null; //,await Api.GetLive(coinName);
//                    _memoryCache.Set($"getLivePrice_{coinName}", result, TimeSpan.FromMinutes(5));
//                }
//                var response = JsonSerializer.Deserialize<CryptoLiveData>(result);
//                var nowTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
//                var duration = nowTimestamp - response.timestamp;
//                if (duration > 600)
//                {
//                    result = null;// await Api.GetLive(coinName);
//                    response = JsonSerializer.Deserialize<CryptoLiveData>(result);
//                    _memoryCache.Remove($"getLivePrice_{coinName}");
//                    _memoryCache.Set($"getLivePrice_{coinName}", result, TimeSpan.FromMinutes(5));
//                }
//                await _hubContext.Clients.User(userCoin.UserId).SendAsync("RecieveLivePrice", response);
//            }
//        }
//        catch (Exception ex)
//        {
//            await _hubContext.Clients.User(userCoin.UserId).SendAsync("ReceiveError", $"Error on get live price of {userCoin.Coin}");
//        }
//    }
//    public void Dispose()
//    {
//        _timer?.Dispose();
//        _getLlivePriceTimer?.Dispose();
//    }
//}
//#endregion
