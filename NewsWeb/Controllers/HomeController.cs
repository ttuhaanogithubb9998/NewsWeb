using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWeb.Models;
using NewsWeb.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NewsWeb.Controllers
{
    public class HomeController : DataUserNavbarController
    {

        public HomeController(NewsWebContext context):base(context)
        {
        }

        public IActionResult Index()
        {
            User_navbar();

            int t = _context.Posts.ToList().Count();
            Random ran = new Random();
            int index = ran.Next(t - 10, t);
            if (index < 6) { index = 6; }
            var listPosts = _context.Posts.Skip(index - 6).Take(5);

            return View(listPosts);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
