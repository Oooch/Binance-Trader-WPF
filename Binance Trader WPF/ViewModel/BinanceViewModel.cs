using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

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
            private decimal _lowPercentChange;
            public decimal LowPercentChange
            {
                get => _lowPercentChange;
                set => SetProperty(ref _lowPercentChange, value);
            }
            private decimal _highPercentChange;
            public decimal HighPercentChange
            {
                get => _highPercentChange;
                set => SetProperty(ref _highPercentChange, value);
            }
            private decimal _lastPrice;
            public decimal LastPrice
            {
                get => _lastPrice;
                set => SetProperty(ref _lastPrice, value);
            }
            private decimal _lowPrice;
            public decimal LowPrice
            {
                get => _lowPrice;
                set => SetProperty(ref _lowPrice, value);
            }
            private decimal _highPrice;
            public decimal HighPrice
            {
                get => _highPrice;
                set => SetProperty(ref _highPrice, value);
            }
            private int _trades;
            public int Trades
            {
                get => _trades;
                set => SetProperty(ref _trades, value);
            }            
            private DateTime _lastUpdated;
            public DateTime LastUpdated
            {
                get => _lastUpdated;
                set => SetProperty(ref _lastUpdated, value);
            }
            private bool _detailedInfo;
            public bool DetailedInfo
            {
                get => _detailedInfo;
                set => SetProperty(ref _detailedInfo, value);
            }
            //private decimal _priceChange1H;
            //public decimal PriceChange1H
            //{
            //    get => _priceChange1H;
            //    set => SetProperty(ref _priceChange1H, value);
            //}
        }

        private ObservableCollection<Coin> _coinsView;
        public ObservableCollection<Coin> CoinsView
        {
            get => _coinsView;
            set => SetProperty(ref _coinsView, value);
        }

        private CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.Binance24HPrice[]> _coins24HPrice;
        public CryptoExchange.Net.Objects.WebCallResult<Binance.Net.Objects.Binance24HPrice[]> Coins24HPrice
        {
            get => _coins24HPrice;
            set => SetProperty(ref _coins24HPrice, value);
        }

        private string _searchPair;
        public string SearchPair
        {
            get => _searchPair;
            set => SetProperty(ref _searchPair, value);
        }

        public async Task<bool> CoinsViewLoad()
        {
            Enabled = false;            
            if (CoinsView == null)
            {
                CoinsView = new ObservableCollection<Coin>();
            }
            while (true)
            {
                Coins24HPrice = BinanceModel.CoinsGet();
                foreach (var coin in Coins24HPrice.Data.OrderBy(o => o.Symbol))
                {
                    if (coin.Symbol.EndsWith("BTC") | coin.Symbol.EndsWith("BNB") | coin.Symbol.EndsWith("ETH") | coin.Symbol.EndsWith("UDST"))
                    {
                        if (coin.Trades > 1000)
                        {
                            //var coinIndepth1M = await BinanceModel.CoinIndepth(coin.Symbol, Binance.Net.Objects.KlineInterval.OneMinute);
                            //var coinIndepth1H = await BinanceModel.CoinIndepth(coin.Symbol, Binance.Net.Objects.KlineInterval.OneHour);
                            var coinExists = CoinsView.Where(o => o.Currency == coin.Symbol);
                            var newCoin = new Coin();
                            if (!coinExists.Any())
                            {
                                newCoin = new Coin();
                            }
                            else
                            {
                                newCoin = coinExists.First();
                            }
                            newCoin.Currency = coin.Symbol;
                            newCoin.Volume24H = decimal.Round(coin.Volume, 2);
                            newCoin.PriceChange24H = coin.PriceChangePercent;
                            newCoin.LowPrice = coin.LowPrice;
                            newCoin.HighPrice = coin.HighPrice;
                            newCoin.LastPrice = coin.LastPrice;
                            if (coin.LastPrice > 0)
                            {
                                newCoin.LowPercentChange = ((coin.LowPrice - coin.LastPrice) / coin.LastPrice * 100);
                                newCoin.LowPercentChange = Math.Round(newCoin.LowPercentChange, 2);
                                newCoin.HighPercentChange = ((coin.HighPrice - coin.LastPrice) / coin.LastPrice * 100);
                                newCoin.HighPercentChange = Math.Round(newCoin.HighPercentChange, 2);
                            }
                            newCoin.Trades = coin.Trades;
                            newCoin.LastUpdated = DateTime.Now;

                            if (!coinExists.Any())
                            {
                                CoinsView.Add(newCoin);
                                RaisePropertyChanged(() => CoinsView);
                            }                            
                        }
                    }
                }
                await Task.Delay(15000);                
            }
        }

        public void FilterCoin(ICollectionView view)
        {
            if (view != null)
            {
                if (!string.IsNullOrEmpty(SearchPair))
                {
                    view.Filter = o =>
                    {
                        var coin = o as Coin;
                        return coin.Currency.StartsWith(SearchPair, StringComparison.InvariantCultureIgnoreCase);
                    };
                }
                else
                {
                    view.Filter = null;
                }
                RaisePropertyChanged(() => CoinsView);
                view.Refresh();
            }
        }

        public void RateLimts()
        {
            BinanceModel.RateLimts();
        }

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
                    ObservableCollection<BinanceViewModel.Coin> CoinView = value as ObservableCollection<BinanceViewModel.Coin>;
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

    public class ColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    decimal PercentValue = (decimal)value;
                    if(PercentValue > 0 & PercentValue < 10)
                    {
                        return Brushes.LightGreen;
                    }
                    if(PercentValue >= 10)
                    {
                        return Brushes.Green;
                    }
                    if(PercentValue < 0)
                    {
                        return Brushes.Red;
                    }
                    if(PercentValue < 10)
                    {
                        return Brushes.DarkRed;
                    }
                }
                return Brushes.White;
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
}
