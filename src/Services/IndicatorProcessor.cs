using Bitget.Net.Objects.Models.V2;
using IndicatorBot.Helpers;
using Skender.Stock.Indicators;

namespace IndicatorBot.Services;

public static class IndicatorProcessor
{
    public static IEnumerable<SuperTrendResult> CalculateSuperTrend(IEnumerable<BitgetKline> klines, int atrPeriod, double multiplier)
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
        IEnumerable<BitgetKline> klines,
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

    private static (BitgetKline? lastKline, SuperTrendResult? lastIndicator) ValidateInputs(
        IEnumerable<BitgetKline> klines,
        IEnumerable<SuperTrendResult> superTrendResults)
    {
        var lastKline = klines.Last();
        var lastIndicator = superTrendResults.Last();

        if (lastIndicator.SuperTrend != null) return (lastKline, lastIndicator);
        
        LogHelper.Log("No valid SuperTrend result for analysis.");
        return (null, null);
    }
}