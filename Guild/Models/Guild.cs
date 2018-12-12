using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.Models
{
    [Table("Guilds")]
    public class Guild
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }

    }
}
