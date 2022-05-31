using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWeb.Data;
using NewsWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NewsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : CheckController
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountsController(NewsWebContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _webHostEnvironment = webHostEnvironment;
        }



        // login admin
        public IActionResult Login()
        {
            if (CheckAdmin() != null) { return RedirectToAction("Index", "Accounts"); }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName == UserName && a.Password == Password && a.IsAdmin == true);
            if (account == null)
            {
                ViewBag.Msg = "Thất bại";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("UserName", UserName);
                return RedirectToAction("index", "Accounts");
            }

        }

        // top comments
        public async Task<IActionResult> Comments()
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            // truy vấn
            var listAccounts = await _context.Accounts.Include(a => a.Comments).OrderByDescending(a => a.Comments.Count()).Skip(0).Take(10).ToListAsync();
            return View(listAccounts);
        }

        // top Like
        public IActionResult ManyLikes()
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            // truy vấn
            //var listAccounts = await _context.Accounts.Include(a => a.Comments).Select(a => new
            //{
            //    a = a,
            //    sumLike = a.Comments
            //    .Select(f => new { like = f.Favorites.Where(f => f.State == true).Count() }).Sum(s=>s.like)
            //}).OrderByDescending(s => s.sumLike).Select(a => a.a)
            //    .Skip(0).Take(10).ToListAsync();

            var selectList = _context.Accounts.Include(a => a.Comments).Join(_context.Comments, a => a.Id, c => c.AccountId, (a, c) => new
            {
                a = a,
                CommentId = c.Id
            }).AsEnumerable().Join(_context.Favorites, ac => ac.CommentId, f => f.CommentId, (ac, f) => new
            {
                a = ac.a,
                state = f.State
            }).Where(acf => acf.state == true).GroupBy(acf => acf.a).Select(acf => new
            {
                a = acf.Key,
                like = acf.Count()
            }).OrderByDescending(acf => acf.like).ToList();

            var listLikes = selectList.Select(acf => acf.like).ToList();
            ViewBag.listLikes = listLikes;

            var listAccounts = selectList.Select(s => s.a);
            return View(listAccounts);
        }

        public IActionResult ManyDislikes()
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }


            var selectList = _context.Accounts.Include(a => a.Comments).Join(_context.Comments, a => a.Id, c => c.AccountId, (a, c) => new
            {
                a = a,
                CommentId = c.Id
            }).AsEnumerable().Join(_context.Favorites, ac => ac.CommentId, f => f.CommentId, (ac, f) => new
            {
                a = ac.a,
                state = f.State
            }).Where(acf => acf.state == false).GroupBy(acf => acf.a).Select(acf => new
            {
                a = acf.Key,
                like = acf.Count()
            }).OrderByDescending(acf => acf.like).ToList();

            var listLikes = selectList.Select(acf => acf.like).ToList();
            ViewBag.listLikes = listLikes;

            var listAccounts = selectList.Select(s => s.a);
            return View(listAccounts);
        }



        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            return View(await _context.Accounts.ToListAsync());
        }

        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }
            ViewBag.navbar = "Accounts";

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,Email,FullName,Address,IsAdmin,Phone,Avatar,AvatarFile")] Account account)
        {
            ViewBag.navbar = "Accounts";
            if (ModelState.IsValid)
            {
                bool check = _context.Accounts.Any(a => a.UserName == account.UserName || account.Email == a.Email || a.Phone == account.Phone);
                if (check)
                {
                    ViewBag.Msg = "Tên Đăng nhập, email, sdt đã có người sử dụng !";
                    return View();
                }
                else
                {

                    _context.Add(account);
                    await _context.SaveChangesAsync();

                    if (account.AvatarFile != null)
                    {
                        var fileName = account.Id.ToString() + Path.GetExtension(account.AvatarFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "image", "avatar");
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            account.AvatarFile.CopyTo(fs);
                            fs.Flush();
                        }
                        account.Avatar = fileName;
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }
                }


                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,Email,FullName,Address,IsAdmin,Phone,Avatar,AvatarFile")] Account account)
        {
            ViewBag.navbar = "Accounts";
            if (id != account.Id)
            {
                return NotFound();
            }
            bool check = _context.Accounts.Any(a => a.UserName == account.UserName || account.Email == a.Email || a.Phone == account.Phone);
            if (check)
            {
                ViewBag.Msg = "Tên Đăng nhập, email, sdt đã có người sử dụng !";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (account.AvatarFile != null)
                        {
                            string fileName = account.Id.ToString() + Path.GetExtension(account.AvatarFile.FileName);
                            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "image", "avatar");
                            string filePath = Path.Combine(uploadPath, fileName);
                            using (FileStream fs = System.IO.File.Create(filePath))
                            {
                                account.AvatarFile.CopyTo(fs);
                                fs.Flush();
                            }
                            account.Avatar = fileName;
                        }


                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AccountExists(account.Id))
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
            }


            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.navbar = "Accounts";
            if (CheckAdmin() == null) { return RedirectToAction("Login", "Accounts"); }

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.navbar = "Accounts";
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
