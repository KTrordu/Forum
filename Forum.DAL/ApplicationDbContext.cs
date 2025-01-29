using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forum.Entities;

namespace Forum.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Community> Communities { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostContent> PostContents { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
