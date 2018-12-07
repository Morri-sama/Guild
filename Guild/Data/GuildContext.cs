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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = @"Server = (localdb)\mssqllocaldb; Database = Guild; Trusted_Connection = True; ConnectRetryCount = 0";
            optionsBuilder.UseSqlServer(connection);
        }

        public DbSet<Models.Guild> Guild { get; set; }
        public DbSet<User> AspNetUsers { get; set; }
    }
}
