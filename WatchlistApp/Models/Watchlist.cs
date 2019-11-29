using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WatchlistApp.Models
{
    public class Watchlist
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public List<Stock> Stocks { get; set; }
    }
}
