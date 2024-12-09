using IndicatorBot.Helpers;
using Mexc.Net.Objects.Models.Spot;
using Skender.Stock.Indicators;

namespace IndicatorBot.Services;

public static class IndicatorProcessor
{
    public static IEnumerable<SuperTrendResult> CalculateSuperTrend(IEnumerable<MecxKline> klines, int atrPeriod, double multiplier)
    {
        var quotes = klines.Select(kline => new Quote
        {
            Date = kline.OpenTime,
            Open = kline.OpenPrice,
            High = kline.HighPrice,
            Low = kline.LowPrice,
            Close = kline.ClosePrice,
            Volume = kline.Volume
        });

        return quotes.GetSuperTrend(atrPeriod, multiplier).ToList();
    }

    public static string? ProcessSuperTrendResults(
        IEnumerable<MecxKline> klines,
        IEnumerable<SuperTrendResult> superTrendResults,
        string symbol)
    {
        var (lastKline, lastIndicator) = ValidateInputs(klines, superTrendResults);

        if (lastKline == null || lastIndicator == null)
            return null;

        var action = lastIndicator.LowerBand != null ? "BUY" : lastIndicator.UpperBand != null ? "SELL" : null;
        
        LogHelper.Log($"""
                               Time: {DateTime.UtcNow}
                               Action: {action}
                               Symbol: {symbol}
                               Price: {lastKline.ClosePrice}
                               {(action == "BUY" ? "LowerBand" : "UpperBand")}: {(action == "BUY" ? lastIndicator.LowerBand : lastIndicator.UpperBand)}
                               
                       """);
        return action;
    }

    private static (MecxKline? lastKline, SuperTrendResult? lastIndicator) ValidateInputs(
        IEnumerable<MecxKline> klines,
        IEnumerable<SuperTrendResult> superTrendResults)
    {
        var lastKline = klines.Last();
        var lastIndicator = superTrendResults.Last();

        if (lastIndicator.SuperTrend != null) return (lastKline, lastIndicator);
        
        LogHelper.Log("No valid SuperTrend result for analysis.");
        return (null, null);
    }
}