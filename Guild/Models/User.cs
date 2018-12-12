using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.Models
{
    [Table("Users")]
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public int? GuildId { get; set; }
        

        [ForeignKey("GuildId")]
        public virtual Guild Guild { get; set; }

        public User()
        {

        }
    }
}
