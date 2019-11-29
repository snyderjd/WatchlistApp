using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WatchlistApp.Models
{
    public class Stock
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Ticker { get; set; }
        public List<Watchlist> Watchlists { get; set; }
    }
}
