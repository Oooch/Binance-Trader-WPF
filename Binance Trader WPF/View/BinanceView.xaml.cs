using Binance_Trader_WPF.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Binance_Trader_WPF.View
{
    /// <summary>
    /// Interaction logic for BinanceView.xaml
    /// </summary>
    public partial class BinanceView : Window
    {
        BinanceViewModel BinanceViewModel { get; set; }
        public BinanceView()
        {

            InitializeComponent();

            BinanceViewModel = DataContext as BinanceViewModel;
            BinanceViewModel.RateLimts();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await BinanceViewModel.CoinsViewLoad();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await BinanceViewModel.CoinsViewLoad();
        }

        private void Filter()
        {
            if (MainView.ItemsSource is ObservableCollection<BinanceViewModel.Coin> Coins)
            {
                var view = CollectionViewSource.GetDefaultView(MainView.ItemsSource);
                BinanceViewModel.FilterCoin(view);
            }
        }

        private void SearchPairText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void ListView_Sort(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    }

                    string header = headerClicked.Column.Header as string;
                    header = Regex.Replace(header, @"\s+", "");
                    Sort(header, direction);

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lv.ItemsSource);
            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }    
}
