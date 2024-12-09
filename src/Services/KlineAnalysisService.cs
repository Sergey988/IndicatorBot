using IndicatorBot.Helpers;
using Mexc.Net.Interfaces.Clients;

namespace IndicatorBot.Services;

public class KlineAnalysisService(IMexcRestClient client)
{
    public async Task<string?> AnalyzeKlinesAsync(
        string pair, 
        string interval, 
        int limit, 
        int atrPeriod, 
        double multiplier)
    {
        var klineInterval = KlineIntervalHelper.ParseKlineInterval(interval);

        var klinesResult = await client.SpotApi.ExchangeData.GetKlinesAsync(pair, klineInterval, limit: limit);
        
        if (!klinesResult.Success || klinesResult.Data == null)
        {
            throw new Exception($"Failed to fetch candles: {klinesResult.Error}");
        }

        var superTrendResults = IndicatorProcessor.CalculateSuperTrend(klinesResult.Data, atrPeriod, multiplier);

        var signal = IndicatorProcessor.ProcessSuperTrendResults(klinesResult.Data, superTrendResults, pair);

        if (signal == null)
        {
            LogHelper.Log("No valid signal generated.");
        }

        return signal;
    }
}