using Guild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public int? GuildId { get; set; }
        public bool IsAdmin { get; set; }                  
    }
}
