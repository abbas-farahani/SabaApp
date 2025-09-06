namespace MVC.Areas.Dashboard.Models;

public class CoinSignalVM
{
    public DateTime? Timestamp { get; set; }
    public string? Time { get; set; }
    public double? Price { get; set; }
    public double? Rsi { get; set; }
    public double? Macd { get; set; }
    public double? MacdSignal { get; set; }
    public double? MovingAverage { get; set; }
    public double? UpperBand { get; set; }
    public double? LowerBand { get; set; }
    public bool? BuySignal { get; set; }
    public bool? SellSignal { get; set; }
}
