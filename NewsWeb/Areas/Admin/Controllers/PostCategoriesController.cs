using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWeb.Data;
using NewsWeb.Models;

namespace NewsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostCategoriesController : CheckController
    {

        public PostCategoriesController(NewsWebContext context) : base(context)
        {
        }
        // GET: Admin/PostCategories
        public async Task<IActionResult> Index()
        {
            ViewBag.navbar = "PostCategories";
            return View(await _context.PostCategories.ToListAsync());
        }

        // GET: Admin/PostCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.navbar = "PostCategories";
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // GET: Admin/PostCategories/Create
        public IActionResult Create()
        {
            ViewBag.navbar = "PostCategories";
            return View();
        }

        // POST: Admin/PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] PostCategory postCategory)
        {
            ViewBag.navbar = "PostCategories";
            if (ModelState.IsValid)
            {
                _context.Add(postCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(postCategory);
        }

        // GET: Admin/PostCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.navbar = "PostCategories";
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            return View(postCategory);
        }

        // POST: Admin/PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PostCategory postCategory)
        {
            ViewBag.navbar = "PostCategories";
            if (id != postCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategory.Id))
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
            return View(postCategory);
        }

        // GET: Admin/PostCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.navbar = "PostCategories";
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // POST: Admin/PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.navbar = "PostCategories";
            var postCategory = await _context.PostCategories.FindAsync(id);
            _context.PostCategories.Remove(postCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategories.Any(e => e.Id == id);
        }
    }
}
