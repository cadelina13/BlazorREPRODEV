using BlazorREPRODEV.App.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorREPRODEV.App.Server.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : Controller
    {
        public readonly IDbContextFactory<ApplicationDbContext> ctx;
        public readonly IWebHostEnvironment env;
        public AdminController(IDbContextFactory<ApplicationDbContext> _ctx, IWebHostEnvironment _env)
        {
            ctx = _ctx;
            env = _env;
        }
        [HttpGet("GetAnnouncements")]
        public List<AdminAnnouncement> GetAnnouncements()
        {
            var list = new List<AdminAnnouncement>();
            using (var db = ctx.CreateDbContext())
            {
                list = db.AdminAnnouncements.OrderByDescending(x => x.DateCreated).ToList();
            }
            return list;
        }
        [HttpGet("GetUsers")]
        public List<UserAccount> GetUsers()
        {
            var list = new List<UserAccount>();
            using (var db = ctx.CreateDbContext())
            {
                list = db.UserAccounts.Where(x => x.Username != "superadmin").OrderByDescending(x=>x.DateRegistered).ToList();
            }
            return list;
        }
        [HttpPost("UpdateUser")]
        public UserAccount UpdateUser(UserAccount data)
        {
            using (var db = ctx.CreateDbContext())
            {
                db.UserAccounts.Update(data);
                db.SaveChanges();
            }
            return data;
        }
    }
}
