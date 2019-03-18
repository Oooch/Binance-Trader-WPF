using Binance.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_Trader_WPF.Model
{
    public class BinanceModel
    {
        BinanceClient client;
        public CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.Binance24HPrice[]> CoinsGet()
        {
            using (client = new BinanceClient())
            {
                return client.Get24HPricesList();
            }
        }

        public async Task<CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.BinanceKline[]>> CoinIndepth(string Symbol, Binance.Net.Objects.KlineInterval interval)
        {
            var coins = CoinsGet();

            using (client = new BinanceClient())
            {
                 return await client.GetKlinesAsync(Symbol, interval);
            }            
        }

        public async Task<bool> BinanceSocket()
        {
            var binanceStreamTick = new Binance.Net.Objects.BinanceStreamTick();

            using (var client = new BinanceSocketClient())
            {
                var success = client.SubscribeToSymbolTickerAsync("LSKBTC", data => { binanceStreamTick = data; });
                success.Start();
                var result = success.Result;
                await client.UnsubscribeAll();                
            }
            return true;
        }

        public void RateLimts()
        {
            using (client = new BinanceClient())
            {
                var pingResponse = client.Ping();
                if (pingResponse.Error != null)
                {
                    throw new ArgumentException("Binance Ping Issue, fix before continuing");
                }
            }
        }

        //public async Task<CoinMarketCap.Entities.TickerEntity> CoinMarketCapInfo(string Symbol)
        //{
        //    using (var client = new CoinMarketCap.CoinMarketCapClient())
        //    {
        //        return await client.GetGlobalDataAsync(CoinMarketCap.Enums.ConvertEnum.USD);                
        //    }
        //}
    }
}
