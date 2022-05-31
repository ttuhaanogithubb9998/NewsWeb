using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsWeb.Models;
using Microsoft.AspNetCore.Http;
using NewsWeb.Data;
namespace NewsWeb.Areas.Admin.Controllers
{
    public class CheckController : Controller
    {
        public readonly NewsWebContext _context;

        public CheckController( NewsWebContext context)
        {
            _context = context;
        }
        public Account CheckAdmin()
        {
            string username = HttpContext.Session.GetString("UserName");
            var account = _context.Accounts.Where(a => a.IsAdmin == true && a.UserName == username).FirstOrDefault();
            return account;
        }
    }
}
