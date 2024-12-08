﻿using EnvironmentManager.Extensions;

namespace IndicatorBot.Models;

public class EnvConfig
{
    public int KlineLimit { get; } = Env.KlineLimit.Get<int>();
    public string KlineInterval { get; } = Env.KlineInterval.Get<string>();
    public int SuperTrendAtrPeriod { get; } = Env.SuperTrendAtrPeriod.Get<int>();
    public int SuperTrendMultiplier { get; } = Env.SuperTrendMultiplier.Get<int>();
    public string SellSymbol { get; } = Env.SellSymbol.Get<string>();
    public string BuySymbol { get; } = Env.BuySymbol.Get<string>();
    public string Pair { get; } = Env.Pair.Get<string>();
    public bool Logger { get; } = Env.Logger.Get<bool>();
    public string ApiKey { get; } = Env.ApiKey.Get<string>();
    public string SecretKey { get; } = Env.SecretKey.Get<string>();
    public string Passphrase { get; } = Env.Passphrase.Get<string>();
}