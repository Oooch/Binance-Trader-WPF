using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Binance_Trader_WPF.ViewModel
{    
    class BinanceViewModel : ViewModel
    {
        Model.BinanceModel BinanceModel = new Model.BinanceModel();

        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled, value);
        }

        public class Coin : ViewModel
        {
            public string Currency { get; set; }
            //private decimal _candles03;
            //public decimal Candles03
            //{
            //    get => _candles03;
            //    set => SetProperty(ref _candles03, value);
            //}
            //private decimal _candles36;
            //public decimal Candles36
            //{
            //    get => _candles36;
            //    set => SetProperty(ref _candles36, value);
            //}
            //private decimal _candles69;
            //public decimal Candles69
            //{
            //    get => _candles69;
            //    set => SetProperty(ref _candles69, value);
            //}
            private decimal _volume24H;
            public decimal Volume24H
            {
                get => _volume24H;
                set => SetProperty(ref _volume24H, value);
            }
            private decimal _priceChange24H;
            public decimal PriceChange24H
            {
                get => _priceChange24H;
                set => SetProperty(ref _priceChange24H, value);
            }
            //private decimal _priceChange1H;
            //public decimal PriceChange1H
            //{
            //    get => _priceChange1H;
            //    set => SetProperty(ref _priceChange1H, value);
            //}
            //private int _marketCap;
            //public int MarketCap
            //{
            //    get => _marketCap;
            //    set => SetProperty(ref _marketCap, value);
            //}
            private DateTime _lastUpdated;
            public DateTime LastUpdated
            {
                get => _lastUpdated;
                set => SetProperty(ref _lastUpdated, value);
            }
        }

        private ObservableCollection<Coin> _coinsView;
        public ObservableCollection<Coin> CoinsView
        {
            get => _coinsView;
            set => SetProperty(ref _coinsView, value);
        }

        public async Task<bool> CoinsViewLoad()
        {
            var coins = BinanceModel.CoinsGet();
            if(CoinsView == null)
            {
                CoinsView = new ObservableCollection<Coin>();
            }
            while (true)
            {
                foreach (var coin in coins.Data.OrderBy(o => o.Symbol))
                {
                    if (coin.Volume > 100 && coin.Symbol == "ARNBTC")
                    {
                        var coinExists = CoinsView.Where(o => o.Currency == coin.Symbol);
                        if (coinExists == null)
                        {
                            //var coinIndepth1M = await BinanceModel.CoinIndepth(coin.Symbol, Binance.Net.Objects.KlineInterval.OneMinute);
                            //var coinIndepth1H = await BinanceModel.CoinIndepth(coin.Symbol, Binance.Net.Objects.KlineInterval.OneHour);
                            var newCoin = new Coin();
                            newCoin.Currency = coin.Symbol;
                            newCoin.Volume24H = coin.Volume;
                            //newCoin.Candles03 = 
                            newCoin.PriceChange24H = coin.PriceChangePercent;
                            newCoin.LastUpdated = DateTime.Now;
                            CoinsView.Add(newCoin);
                            RaisePropertyChanged(() => CoinsView);
                        }
                    }
                }
                await Task.Delay(60000);                
            }
        }

        public void RateLimts()
        {
            BinanceModel.RateLimts();
        }

        //public class CoinDBData : ViewModel
        //{
        //    private DbSet<Data.Coin> _coins;
        //    public DbSet<Data.Coin> Coins
        //    {
        //        get => _coins;
        //        set => SetProperty(ref _coins, value);
        //    }
        //    private DbSet<Data.CoinValue> _coinValues;
        //    public DbSet<Data.CoinValue> CoinValues
        //    {
        //        get => _coinValues;
        //        set => SetProperty(ref _coinValues, value);
        //    }
        //}

        //private ObservableCollection<CoinDBData> _coinDBDataObservable;
        //public ObservableCollection<CoinDBData> CoinDBDataObservable
        //{
        //    get => _coinDBDataObservable;
        //    set => SetProperty(ref _coinDBDataObservable, value);
        //}

        //public void UpdateDB()
        //{
        //    BinanceModel.CoinsUpdate();
        //}

        //public void LoadDbSets()
        //{
        //    using (var db = new Data.CoinDbContext())
        //    {
        //        var coinDBData = new CoinDBData();
        //        db.Coins.Load();
        //        coinDBData.Coins = db.Coins;
        //        CoinDBDataObservable = new ObservableCollection<CoinDBData>();
        //        CoinDBDataObservable.Add(coinDBData);
        //        RaisePropertyChanged(() => CoinDBDataObservable);
        //    }
        //}


        public async Task<bool> BinanceSocket()
        {
            await BinanceModel.BinanceSocket();
            return true;
        }
    }

    public class CoinsViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    var CoinView = value as ObservableCollection<BinanceViewModel.Coin>;
                    if (CoinView == null || !CoinView.Any())
                    {
                        return null;
                    }
                    return CoinView;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class CoinDBDataToComboBox : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        try
    //        {
    //            if (value != null)
    //            {
    //                var CoinDBData = value as ObservableCollection<BinanceViewModel.CoinDBData>;
    //                if (CoinDBData == null || !CoinDBData.Any())
    //                {
    //                    return null;
    //                }
    //                List<string> coinNames = new List<string>();
    //                foreach (var coin in CoinDBData.First().Coins)
    //                {
    //                    coinNames.Add(coin.Pair);
    //                }
    //                return coinNames;
    //            }
    //            return null;
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
