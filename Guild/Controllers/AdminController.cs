using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guild.Data;
using Guild.Models;
using Guild.UI.Pagination;
using Guild.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Guild.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users(int? page, int? pageSize)
        {
            using(var context = new GuildContext())
            {
                var users = from s in context.AspNetUsers
                            select s;

                return View(await PaginatedList<User>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize ?? 5));
            }
                
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var vm = new CreateUserViewModel();
            vm.Guilds = new List<SelectListItem>();

            using (var context = new GuildContext())
            {
                foreach (Models.Guild x in context.Guild)
                {
                    vm.Guilds.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
            }


            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { FirstName = model.FirstName, MiddleName = model.MiddleName, LastName = model.LastName, GuildId = model.GuildId, UserName=model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            using (var db = new GuildContext())
            {
                foreach (Models.Guild x in db.Guild)
                {
                    model.Guilds.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
            }

            return View(model);
        }
    }
}