using Amazon.Lambda.Core;
using Bitget.Net.Clients;
using Bitget.Net.Objects;
using IndicatorBot.Helpers;
using IndicatorBot.Models;
using IndicatorBot.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace IndicatorBot;

public class Function
{
    public static async Task FunctionHandler()
    {      
        var config = new EnvConfig();

        LogHelper.SetLoggingEnabled(config.Logger);

        try
        {
            var client = new BitgetRestClient();
            
            client.SetApiCredentials(new BitgetApiCredentials(config.ApiKey, config.SecretKey, config.Passphrase));
            
            var signal = await new KlineAnalysisService(client).AnalyzeKlinesAsync(
                pair: config.Pair,
                interval: config.KlineInterval,
                limit: config.KlineLimit,
                atrPeriod: config.SuperTrendAtrPeriod,
                multiplier: config.SuperTrendMultiplier);
            
            if (signal == null)
            {
                LogHelper.Log("No valid signal generated. Skipping further operations.");
                return;
            }
            
            await new OrderPlacementService(client).PlaceOrderAsync(config.Pair, signal, config.SellSymbol, config.BuySymbol);
        }
        catch (Exception ex)
        {
            LogHelper.Log($"Error: {ex.Message}");
        }
    }
}