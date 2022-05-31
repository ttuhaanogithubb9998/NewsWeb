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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NewsWeb.Controllers
{
    public class AccountsController : DataUserNavbarController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountsController(NewsWebContext context, IWebHostEnvironment webHostEnvironment):base(context)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Accounts
        public  IActionResult Index()
        {
            Account account = User_navbar();
            if (account == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            else
            {
                return View( account);
            }
        }


        //Get: accounts/Login
        public IActionResult Login()
        {

            Account account = User_navbar();


            if (account == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //Post: account/login
        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        {
            User_navbar();

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName == UserName && a.Password == Password);
            if (account == null)
            {
                ViewBag.Msg = "Thất bại";
                return View();
            }
            else
            {
                HttpContext.Response.Cookies.Append("UserName", account.UserName, new CookieOptions { Expires = DateTime.Now.AddDays(7), });
                return RedirectToAction("Index", "Home");
            }
        }
        //
        // get//:account/register
        public IActionResult Register()
        {
            User_navbar();

            return View();
        }

        // post/account/register

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,UserName,Password,FullName,Email,Address,Avatar,Phone,AvatarFile")] Account account)
        {
            User_navbar();

            bool check = _context.Accounts.Any(a => a.UserName == account.UserName || account.Email == a.Email || a.Phone == account.Phone) ;
            if (!check)
            {
                _context.Accounts.Add(account);
                _context.SaveChanges();

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
                    _context.Accounts.Update(account);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Response.Cookies.Append("UserName", account.UserName, new CookieOptions { Expires = DateTime.Now.AddDays(7) });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Msg = "Tên Đăng nhập, email, sdt đã có người sử dụng !";
                return View();
            }
        }


        // logout
        public IActionResult Logout()
        {
            User_navbar();

            HttpContext.Response.Cookies.Delete("UserName");
            return RedirectToAction("index", "Home");
        }

        // get account comments
        public async Task<IActionResult> Comments()
        {
            User_navbar();

            string username = HttpContext.Request.Cookies["UserName"];
            if (username != null)
            {
                int accountId = _context.Accounts.FirstOrDefault(a => a.UserName == username).Id;
                var listComment = await _context.Comments.Where(c => c.AccountId == accountId).ToListAsync();

                return View(listComment);
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }

        //get savedPost
        public async Task<IActionResult> SavedPosts()
        {

            var account = User_navbar();

            if (account == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            else
            {
                int accountId = account.Id;
                
                //truy vấn 
                var selectList = await _context.Posts.Join(_context.SavedPosts, p => p.Id, s => s.PostId, (p, s) => new
                {
                    p = p,
                    s = s
                })
                    .Where(ps => ps.s.AccountId == accountId).ToListAsync(  );

                // convert bảng truy vấn được sang list
                List<Post> listPosts = new List<Post>();

                foreach (var sl in selectList)
                {
                    Post post = new Post();

                    post.Id = sl.p.Id;
                    post.Title = sl.p.Title;
                    post.Imgage = sl.p.Imgage;
                    post.Content = sl.p.Content;

                    listPosts.Add(post);
                }

                    return View(listPosts);
            }
        }


        // edit
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id","UserName","Password","Email","Phone","Address","FullName","Avatar","AvatarFile")] Account account)
        {
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

            }

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Accounts");
        }
        



        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
