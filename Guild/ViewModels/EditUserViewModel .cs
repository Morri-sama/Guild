using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.ViewModels
{
    public class EditUserViewModel
    {
        [Display(Name = "ID пользователя")]
        public string UserId {get;set;}

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Цех")]
        public int GuildId { get; set; }

        public List<SelectListItem> Guilds { get; set; }

        [Display(Name = "Администратор")]
        public bool IsAdmin { get; set; }

        public EditUserViewModel()
        {

        }
    }
}
