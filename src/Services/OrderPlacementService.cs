using IndicatorBot.Helpers;
using IndicatorBot.Models;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients;

namespace IndicatorBot.Services;

public class OrderPlacementService(IMexcRestClient client)
{
    public async Task PlaceOrderAsync(EnvConfig config, string signal)
    {
        var orderAmount = await GetValidOrderAmountAsync(config, signal);
    
        if (orderAmount <= 0)
        {
            LogHelper.Log($"Insufficient balance for {signal} signal on {config.Pair}.");
            return;
        }

        LogHelper.Log($"Signal received: {signal}. Order Amount: {orderAmount} on {config.Pair}");

        await PlaceMarketOrderAsync(config.Pair, signal, orderAmount);
    }

    private async Task<decimal> GetValidOrderAmountAsync(EnvConfig config, string signal)
    {
        var accountBalanceResult = await client.SpotApi.Account.GetAccountInfoAsync();

        if (!accountBalanceResult.Success || accountBalanceResult.Data == null)
        {
            throw new Exception($"Failed to fetch balance: {accountBalanceResult.Error}");
        }

        var baseBalance = accountBalanceResult.Data.Balances.FirstOrDefault(b => b.Asset == config.SellSymbol)?.Available ?? 0;
        var quoteBalance = accountBalanceResult.Data.Balances.FirstOrDefault(b => b.Asset == config.BuySymbol)?.Available ?? 0;

        LogHelper.Log($"Available Balance: {config.SellSymbol} - {baseBalance}, {config.BuySymbol} - {quoteBalance}.");
    
        return signal switch
        {
            "BUY" => quoteBalance < config.MinBuyOrderAmount ? 0 : quoteBalance,
            "SELL" => baseBalance < config.MinSellOrderAmount ? 0 : baseBalance,
            _ => 0
        };
    }

    private async Task PlaceMarketOrderAsync(string pair, string signal, decimal orderAmount)
    {
        var orderResult = await client.SpotApi.Trading.PlaceOrderAsync(
            symbol: pair,
            side: signal == "BUY" ? OrderSide.Buy : OrderSide.Sell,
            type: OrderType.Market,
            quantity: signal == "SELL" ? orderAmount : null,
            quoteQuantity: signal == "BUY" ? orderAmount : null);

        if (!orderResult.Success)
        {
            throw new Exception($"Failed to place market order: {orderResult.Error}");
        }

        LogHelper.Log($"Order placed successfully: {signal} {orderAmount} {pair}. Order ID: {orderResult.Data.OrderId}");
    }
}