using Bitget.Net.Enums.V2;
using Bitget.Net.Interfaces.Clients;
using IndicatorBot.Helpers;

namespace IndicatorBot.Services;

public class OrderPlacementService(IBitgetRestClient client)
{
    public async Task PlaceOrderAsync(string pair, string signal, string sellSymbol, string buySymbol)
        {
            var accountBalanceResult = await client.SpotApiV2.Account.GetSpotBalancesAsync();

            if (!accountBalanceResult.Success || accountBalanceResult.Data == null)
            {
                throw new Exception($"Failed to fetch balance: {accountBalanceResult.Error}");
            }

            var baseBalance = accountBalanceResult.Data.FirstOrDefault(b => b.Asset == sellSymbol);
            var quoteBalance = accountBalanceResult.Data.FirstOrDefault(b => b.Asset == buySymbol);

            LogHelper.Log($"Balance: {sellSymbol}-{baseBalance}, {buySymbol}-{quoteBalance}.");
            
            // TODO: min order amount ???
            var orderAmount = signal switch
            {
                "BUY" => quoteBalance?.Available ?? 0,
                "SELL" => baseBalance?.Available ?? 0,
                _ => 0
            };

            if (orderAmount <= 0)
            {
                LogHelper.Log($"Insufficient balance for {signal} signal on {pair}.");
                return;
            }

            LogHelper.Log($"Signal received: {signal}. Order Amount: {orderAmount} {pair}");

            var orderSide = signal == "BUY" ? OrderSide.Buy : OrderSide.Sell;
            const OrderType orderType = OrderType.Market;
            const TimeInForce timeInForce = TimeInForce.GoodTillCanceled;

            var orderResult = await client.SpotApiV2.Trading.PlaceOrderAsync(
                symbol: pair,
                side: orderSide,
                type: orderType,
                quantity: orderAmount,
                timeInForce: timeInForce);

            if (!orderResult.Success)
            {
                throw new Exception($"Failed to place market order: {orderResult.Error}");
            }

            LogHelper.Log($"Order placed successfully: {signal} {orderAmount} {pair}. Order ID: {orderResult.Data.OrderId}");
        }
    }