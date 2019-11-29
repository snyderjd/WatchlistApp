using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WatchlistApp.Models
{
    public class WatchlistStock
    {
        public int Id { get; set; }
        [Required]
        public int StockId { get; set; }
        public Stock Stock { get; set; }
        [Required]
        public int WatchlistId { get; set; }
        public Watchlist Watchlist { get; set; }
    }
}
