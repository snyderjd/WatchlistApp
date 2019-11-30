using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WatchlistApp.Data;
using WatchlistApp.Models;

namespace WatchlistApp.Controllers
{
    public class WatchlistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public WatchlistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Watchlists
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.Watchlists
                .Where(w => w.ApplicationUserId == user.Id)
                .Include(w => w.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Watchlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (watchlist == null)
            {
                return NotFound();
            }

            return View(watchlist);
        }

        // GET: Watchlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Watchlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ApplicationUserId")] Watchlist watchlist)
        {
            var user = await GetCurrentUserAsync();

            //// make necessary join table entries to add selected stocks to the watchlist
            //using (SqlConnection conn = Connection)
            //{
            //    conn.Open();
            //    using (SqlCommand cmd = conn.CreateCommand())
            //    {
            //        foreach(int stockId in watchlist.StockIds)
            //        {
            //            cmd.Parameters.Clear();
            //            cmd.CommandText = @"INSERT INTO WatchlistStock(StockId, WatchlistId)
            //                                VALUES (@stockId, @watchlistId)";
            //            cmd.Parameters.Add(new SqlParameter("@stockId", stockId));
            //            cmd.Parameters.Add(new SqlParameter("@watchlistId", watchlist.Id));
            //            cmd.ExecuteNonQuery();
            //        }
            //    }
            //}

            if (ModelState.IsValid)
            {
                watchlist.ApplicationUserId = user.Id;
                _context.Add(watchlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(watchlist);
        }

        // GET: Watchlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var watchlist = await _context.Watchlists.FindAsync(id);
            if (watchlist == null) return NotFound();

            var stocks = _context.Stocks;
            ViewData["Stocks"] = new SelectList(stocks, "Id", "Name");
            return View(watchlist);
        }

        // POST: Watchlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ApplicationUserId, Stocks, StockIds")] Watchlist watchlist)
        {
            var user = await GetCurrentUserAsync();

            if (id != watchlist.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    watchlist.ApplicationUserId = user.Id;

                    // make necessary join table entries to add selected stocks to the watchlist
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            foreach (int stockId in watchlist.StockIds)
                            {
                                cmd.CommandText = @"INSERT INTO WatchlistStock(StockId, WatchlistId)
                                                    VALUES (@stockId, @watchlistId)";
                                cmd.Parameters.Add(new SqlParameter("@stockId", stockId));
                                cmd.Parameters.Add(new SqlParameter("@watchlistId", watchlist.Id));
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                    _context.Update(watchlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WatchlistExists(watchlist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", watchlist.ApplicationUserId);
            return View(watchlist);
        }

        // GET: Watchlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists
                .Include(w => w.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (watchlist == null)
            {
                return NotFound();
            }

            return View(watchlist);
        }

        // POST: Watchlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var watchlist = await _context.Watchlists.FindAsync(id);
            _context.Watchlists.Remove(watchlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WatchlistExists(int id)
        {
            return _context.Watchlists.Any(e => e.Id == id);
        }
    }
}
