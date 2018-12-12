using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.Models
{
    [Table("Orders")]
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string AdditionalText1 { get; set; }
        public string AdditionalText2 { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Order()
        {

        }
    }
}
