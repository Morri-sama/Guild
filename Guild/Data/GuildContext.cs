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
            //var connection = @"Server = (localdb)\mssqllocaldb; Database = Guild; Trusted_Connection = True; ConnectRetryCount = 0";
            
            var connection = @"Data Source=guilddbserver.database.windows.net;Initial Catalog=Guild;User ID=morri;Password=BeNDeR1488;Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            optionsBuilder.UseSqlServer(connection);
        }


        public DbSet<Models.Guild> Guild { get; set; }
        public DbSet<User> AspNetUsers { get; set; }
    }
}
