using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance_Trader_WPF.Data
{
    public class CoinDbContext : DbContext
    {
        public DbSet<Coin> Coins { get; set; }
        public DbSet<CoinValue> CoinValues { get; set; }  
    }

    public class Coin
    {
        public int Id { get; set; }
        public string Pair { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class CoinValue
    {
        public int Id { get; set; }
        public string Pair { get; set; }
        public DateTime Updated { get; set; }
        public decimal AskPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal Volume { get; set; }
    }
}
