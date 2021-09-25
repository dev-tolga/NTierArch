using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NTier.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Data
{
    public class NTierDBContext : IdentityDbContext
    {
        public NTierDBContext(DbContextOptions<NTierDBContext> options) : base(options)
        {

        }


        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Blog)
                .WithMany(b => b.Posts);

            base.OnModelCreating(modelBuilder);
        }
    }
}
