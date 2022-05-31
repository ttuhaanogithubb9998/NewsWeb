using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsWeb.Data;
using Microsoft.AspNetCore.Http;
using NewsWeb.Models;

namespace NewsWeb.Controllers
{
    public class DataUserNavbarController : Controller
    {
        public readonly NewsWebContext _context;

        public DataUserNavbarController(NewsWebContext context)
        {
            _context = context;
        }

        public Account User_navbar()
        {
            string username = HttpContext.Request.Cookies["Username"];
            var account = _context.Accounts.FirstOrDefault(a => a.UserName == username);
            ViewBag.account = account;

            var postCategories = _context.PostCategories.ToList();
            ViewBag.postCategories = postCategories;

            return account;
        }


    }
}
