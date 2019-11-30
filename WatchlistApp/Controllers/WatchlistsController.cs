using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WatchlistApp.Data;
using WatchlistApp.Models;

namespace WatchlistApp.Controllers
{
    public class WatchlistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WatchlistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> Details(string id)
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Watchlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ApplicationUserId")] Watchlist watchlist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(watchlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", watchlist.ApplicationUserId);
            return View(watchlist);
        }

        // GET: Watchlists/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists.FindAsync(id);
            if (watchlist == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", watchlist.ApplicationUserId);
            return View(watchlist);
        }

        // POST: Watchlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ApplicationUserId")] Watchlist watchlist)
        {
            if (id != watchlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
        public async Task<IActionResult> Delete(string id)
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
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var watchlist = await _context.Watchlists.FindAsync(id);
            _context.Watchlists.Remove(watchlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WatchlistExists(string id)
        {
            return _context.Watchlists.Any(e => e.Id == id);
        }
    }
}
