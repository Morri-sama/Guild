using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guild.Data;
using Guild.Models;
using Guild.UI.Pagination;
using Guild.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Guild.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users(int? page, int? pageSize)
        {
            var users = _userManager.Users;
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (var user in users)
            {
                list.Add(new UserViewModel { FirstName = user.FirstName, LastName = user.LastName, MiddleName = user.MiddleName, Role = await _userManager.GetRolesAsync(user), GuildId=user.GuildId, Id=user.Id, UserName=user.UserName });
            }

            using(var context = new GuildContext())
            {
                return View(list);
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


        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                

                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                CreateUserViewModel model = new CreateUserViewModel()
                {
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };

                model.Guilds = new List<SelectListItem>();

                using (var context = new GuildContext())
                {
                    foreach (Models.Guild x in context.Guild)
                    {
                        SelectListItem xd = new SelectListItem();
                        xd.Text = x.Name;
                        xd.Value = x.Id.ToString();
                        model.Guilds.Add(xd);
                    }
                }

                return View(model);
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult CreateGuild()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGuild(GuildViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guild.Models.Guild guild = new Models.Guild() { Name = model.Name };

                using (var db = new GuildContext())
                {
                    await db.AddAsync(guild);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Admin");
                }

            }

            return View(model);
        }
    }
}