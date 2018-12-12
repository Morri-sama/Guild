using Guild.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.Data
{
    public class GuildContext:DbContext
    {
        public GuildContext(DbContextOptions<GuildContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Models.Guild>().ToTable("Guilds", "dbo");
            builder.Entity<Order>().ToTable("Orders", "dbo");
        }

        public DbSet<Models.Guild> Guilds { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
