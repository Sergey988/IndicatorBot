using System.Reflection;
using CryptoExchange.Net.Attributes;
using KlineInterval = Mexc.Net.Enums.KlineInterval;

namespace IndicatorBot.Helpers;

public static class KlineIntervalHelper
{
    public static KlineInterval ParseKlineInterval(string interval)
    {
        foreach (var value in Enum.GetValues<KlineInterval>())
        {
            var memberInfo = typeof(KlineInterval).GetMember(value.ToString()).FirstOrDefault();
            var mapAttribute = memberInfo?.GetCustomAttribute<MapAttribute>();
            if (mapAttribute != null && mapAttribute.Values.Contains(interval))
            {
                return value;
            }
        }

        throw new ArgumentException($"Unsupported interval: {interval}");
    }
}