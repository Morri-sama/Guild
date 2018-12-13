using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Guild.ViewModels
{
    public class EditUserPasswordViewModel
    {
        [Display(Name = "ID пользователя")]
        public string UserId { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName{ get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
