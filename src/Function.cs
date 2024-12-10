using Amazon.Lambda.Core;
using CryptoExchange.Net.Authentication;
using IndicatorBot.Helpers;
using IndicatorBot.Models;
using IndicatorBot.Services;
using Mexc.Net.Clients;

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
            var client = new MexcRestClient();
            
            client.SetApiCredentials(new ApiCredentials(config.ApiKey, config.SecretKey));
            
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

            await new OrderPlacementService(client).PlaceOrderAsync(config, signal);
        }
        catch (Exception ex)
        {
            LogHelper.Log($"Error: {ex.Message}");
        }
    }
}