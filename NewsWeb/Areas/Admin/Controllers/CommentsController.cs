using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWeb.Data;
using NewsWeb.Models;
using Microsoft.AspNetCore.Http;

namespace NewsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentsController : CheckController
    {

        public CommentsController(NewsWebContext context) : base (context)
        {
        }


        //latest comment
        public async Task<IActionResult> Latest()
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listComments = await _context.Comments.Include(c=>c.Post).Include(c=>c.Account).OrderByDescending(c => c.Date).Skip(0).Take(10).ToListAsync();

            return View(listComments);
        }

        //many like
        public async Task<IActionResult> ManyLikes()
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listComments = await _context.Comments.Include(c=>c.Post).Include(c=>c.Account).Include(c=>c.Favorites).OrderByDescending(c => c.Favorites.Where(f=>f.State==true).Count()).Skip(0).Take(10).ToListAsync();

            return View(listComments);
        }

        //many dislike
        public async Task<IActionResult> ManyDislikes()
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listComments = await _context.Comments.Include(c=>c.Post).Include(c=>c.Account).Include(c=>c.Favorites).OrderByDescending(c => c.Favorites.Where(f=>f.State==false).Count()).Skip(0).Take(10).ToListAsync();

            return View(listComments);
        }

        // GET: Admin/Comments
        public async Task<IActionResult> Index()
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var newsWebContext = _context.Comments.Include(c => c.Account).Include(c => c.Post);
            return View(await newsWebContext.ToListAsync());
        }

        // GET: Admin/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Account)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Admin/Comments/Create
        public IActionResult Create()
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "UserName");
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            return View();
        }

        // POST: Admin/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Description,PostId,AccountId")] Comment comment)
        {
            ViewBag.navbar = "Comments";
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "UserName", comment.AccountId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            return View(comment);
        }

        // GET: Admin/Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "UserName", comment.AccountId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            return View(comment);
        }

        // POST: Admin/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Description,PostId,AccountId")] Comment comment)
        {
            ViewBag.navbar = "Comments";
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "UserName", comment.AccountId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", comment.PostId);
            return View(comment);
        }

        // GET: Admin/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.navbar = "Comments";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Account)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.navbar = "Comments";
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
