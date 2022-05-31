using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsWeb.Models;

namespace NewsWeb.Data
{
    public class NewsWebContext : DbContext
    {
        public NewsWebContext(DbContextOptions<NewsWebContext> options ) : base(options) { }

        public DbSet<Account> Accounts { set; get; }

        public DbSet<Comment> Comments { set; get; }

        public DbSet<Favorite> Favorites { set; get; }

        public DbSet<Post> Posts { set; get; }

        public DbSet<PostCategory> PostCategories { set; get; }

        public DbSet<SavedPost> SavedPosts { set; get; }
    
    }
}
