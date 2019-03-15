using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Binance_Trader_WPF.View
{
    /// <summary>
    /// Interaction logic for BinanceView.xaml
    /// </summary>
    public partial class BinanceView : Window
    {
        ViewModel.BinanceViewModel BinanceViewModel { get; set; }
        public BinanceView()
        {
            
            InitializeComponent();

            BinanceViewModel = DataContext as ViewModel.BinanceViewModel;
            //BinanceViewModel.CandleScan();
            //BinanceViewModel.LoadDbSets();
            BinanceViewModel.RateLimts();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await BinanceViewModel.CoinsViewLoad();
        }
    }
}
