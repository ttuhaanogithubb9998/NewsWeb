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
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace NewsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : CheckController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostsController(NewsWebContext context,IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        // latest post
        public async Task<IActionResult> Latest()
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listPosts = await _context.Posts.Include(p => p.PostCategory).OrderByDescending(p => p.Date).Skip(0).Take(10).ToListAsync();

            return View(listPosts);
        }

        // many comment
        public async Task<IActionResult> ManyComments()
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listPosts = await _context.Posts.Include(p => p.PostCategory).Include(p => p.Comments).OrderByDescending(p => p.Comments.Count()).Skip(0).Take(10).ToListAsync();

            return View(listPosts);
        }

        // many views
        public async Task<IActionResult> ManyViews()
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var listPosts = await _context.Posts.OrderByDescending(p => p.View).Skip(0).Take(10).ToListAsync();

            return View(listPosts);
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            var newsWebContext = _context.Posts.Include(p => p.PostCategory);
            return View(await newsWebContext.ToListAsync());
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "Id", "Name");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostCategoryId,Title,Author,Content,Date,Imgage,View,ImageFile")] Post post)
        {
            ViewBag.navbar = "Posts";
            if (ModelState.IsValid)
            {
                if (post.ImageFile != null)
                {
                    string fileName = post.Id.ToString() + Path.GetExtension(post.ImageFile.FileName);
                    string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "image", "post");
                    string filePath = Path.Combine(uploadPath, fileName);
                    using(FileStream fs  = System.IO.File.Create(fileName))
                    {
                        post.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    post.Imgage = fileName;
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "Id", "Id", post.PostCategoryId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "Id", "Id", post.PostCategoryId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostCategoryId,Title,Author,Content,Date,Imgage,View,ImageFile")] Post post)
        {
            ViewBag.navbar = "Posts";
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (post.ImageFile != null)
                    {
                        string fileName = post.Id.ToString() + Path.GetExtension(post.ImageFile.FileName);
                        string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "image", "post");
                        string filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            post.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        post.Imgage = fileName;
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "Id", "Id", post.PostCategoryId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.navbar = "Posts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.navbar = "Posts";
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
