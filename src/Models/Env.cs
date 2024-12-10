using EnvironmentManager.Attributes;

namespace IndicatorBot.Models;

public enum Env
{
    [EnvironmentVariable(typeof(int), isRequired: true)]
    KlineLimit,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    KlineInterval,
    
    [EnvironmentVariable(typeof(int), isRequired: true)]
    SuperTrendAtrPeriod,
    
    [EnvironmentVariable(typeof(int), isRequired: true)]
    SuperTrendMultiplier,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    SellSymbol,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    BuySymbol,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    Pair,
    
    [EnvironmentVariable(typeof(decimal), isRequired: true)]
    MinBuyOrderAmount,
    
    [EnvironmentVariable(typeof(decimal), isRequired: true)]
    MinSellOrderAmount,
    
    [EnvironmentVariable(typeof(bool), isRequired: true)]
    Logger,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    ApiKey,
    
    [EnvironmentVariable(typeof(string), isRequired: true)]
    SecretKey,
    
    [EnvironmentVariable(typeof(int), isRequired: true)]
    BuyOrderDecimals,

    [EnvironmentVariable(typeof(int), isRequired: true)]
    SellOrderDecimals
}