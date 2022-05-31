using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsWeb.Data;
using NewsWeb.Models;

namespace NewsWeb.Controllers
{
    public class PostsController : DataUserNavbarController
    {

        public PostsController(NewsWebContext context):base(context)
        {
        }

        // GET: Posts
        public IActionResult Index()
        {
            return RedirectToAction("PostsLatest");
        }

        // get postCategory/id
        public async Task<IActionResult> PostCategorys(int id)
        {

            User_navbar();


            var list = await _context.Posts.Include(p => p.PostCategory).Where(p => p.PostCategoryId == id).ToListAsync();
            return View(list);
        }


        // get/post latest
        public async Task<IActionResult> PostsLatest()
        {
            User_navbar();
            var listPosts = await _context.Posts.Include(p => p.PostCategory).OrderByDescending(p => p.Date).ToListAsync();
            return View(listPosts);
        }


        //post comment
        [HttpPost]
        public async Task<IActionResult> Comment(string str, int postId)
        {
            string username = HttpContext.Request.Cookies["UserName"];
            if (username != null)
            {
                int accountId = _context.Accounts.FirstOrDefault(a => a.UserName == username).Id;
                DateTime date = DateTime.Now;
                Comment comment = new Comment();
                comment.AccountId = accountId;
                comment.PostId = postId;
                comment.Description = str;
                comment.Date = date;

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }

        }


        //post like comment

        public async Task<IActionResult> Like(int commentId, int state, int postId)
        {
            string username = HttpContext.Request.Cookies["UserName"];
            if (username != null)
            {
                int accountId = _context.Accounts.FirstOrDefault(a => a.UserName == username).Id;
                var f = _context.Favorites.FirstOrDefault(f => f.AccountId == accountId && f.CommentId == commentId);
                if (f != null)
                {
                    // xóa like hoặc dislike khi nhấn lại;
                    if ((f.State == true && state == 1) || (f.State == false && state == 0))
                    {
                        _context.Favorites.Remove(f);
                        await _context.SaveChangesAsync();

                    }
                    // thay đổi like sang dislike và ngược lại
                    else
                    {
                        f.State = !f.State;
                        _context.Favorites.Update(f);
                        await _context.SaveChangesAsync();
                    }
                }
                // tạo favorite khi chưa tương tác
                else
                {
                    Favorite fn = new Favorite();
                    fn.CommentId = commentId;
                    fn.AccountId = accountId;
                    if (state == 0)
                    {
                        fn.State = false;
                    }
                    else
                    {
                        fn.State = true;
                    }

                    _context.Favorites.Add(fn);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Details", "Posts", new { id = postId });
            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }


        // search 
        public async Task<IActionResult> Search(string str)
        {
            User_navbar();

            var listPosts = _context.Posts.Include(p => p.PostCategory).Where(p => p.Title.Contains(str));

            return View(await listPosts.ToListAsync());
        }



        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Account account = User_navbar();
            ViewBag.account = account;

            var post = await _context.Posts
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }


            var selectList = _context.Favorites.Join(_context.Comments, f => f.CommentId, c => c.Id,
                (f, c) => new
                {
                    Id = f.Id,
                    State = f.State,
                    PostId = c.PostId,
                    CommentId = c.Id,
                    AccountId = f.AccountId
                }).Where(fc => fc.PostId == id).ToList();

            List<Favorite> listFavorites = new List<Favorite>();
            foreach(var sl in selectList)
            {
                Favorite f = new Favorite();

                f.Id = sl.Id;
                f.State = sl.State;
                f.CommentId = sl.CommentId;
                f.AccountId = sl.AccountId;
                listFavorites.Add(f);
            }



            var listComments = _context.Comments.Where(c => c.PostId == id).OrderByDescending(c => c.Date).ToList();
            ViewBag.listComments = listComments;
            ViewBag.listFavorites = listFavorites;

            // xem user đã lưu bài viết này hay chưa
            bool userSaved = false;

            if (account != null)
            {
                userSaved = _context.SavedPosts.Where(sp => sp.AccountId == account.Id && sp.PostId == id).Count() > 0;
            }

            ViewBag.userSaved = userSaved;


            post.View++;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return View(post);
        }



        // savePost
        public async Task<IActionResult> SavePost(int postId)
        {
            string username = HttpContext.Request.Cookies["UserName"];
            if (username != null)
            {
                int accountId = _context.Accounts.FirstOrDefault(a => a.UserName == username).Id;

                var savedPost = _context.SavedPosts.FirstOrDefault(sp => sp.AccountId == accountId && sp.PostId == postId);

                // xem bài đã lưu hay chưa
                if (savedPost == null)
                {
                    // lưu

                    SavedPost s = new SavedPost();
                    s.AccountId = accountId;
                    s.PostId = postId;
                    _context.SavedPosts.Add(s);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //xóa lưu
                    _context.SavedPosts.Remove(savedPost);
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction("Details", "Posts", new { id = postId });



            }
            else
            {
                return RedirectToAction("Login", "Accounts");
            }
        }





        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
