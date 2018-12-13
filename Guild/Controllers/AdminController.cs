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
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly GuildContext _context;

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, GuildContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            Guild.Models.Guild guild = _context.Guilds.Include(t => t.Users).FirstOrDefault();

            return View(guild);
        }

        public async Task<IActionResult> Users(int? page, int? pageSize)
        {
            var users = _userManager.Users.Include(x => x.Guild);
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (var user in users)
            {
                var x = new UserViewModel();
                x.FirstName = user.FirstName;
                x.LastName = user.LastName;
                x.MiddleName = user.MiddleName;
                x.IsAdmin = await _userManager.IsInRoleAsync(user, "admin");
                x.GuildId = user.GuildId;
                x.Id = user.Id;
                x.UserName = user.UserName;
                if (user.GuildId != null)
                {
                    x.GuildName = user.Guild.Name;
                }
                else
                {
                    x.GuildName = "Не состоит в цехе";
                }


                list.Add(x);
            }

            return View(list);

        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            var vm = new CreateUserViewModel();
            vm.Guilds = new List<SelectListItem>();

            foreach (Models.Guild x in _context.Guilds)
            {
                vm.Guilds.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }



            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { FirstName = model.FirstName, MiddleName = model.MiddleName, LastName = model.LastName, GuildId = model.GuildId??default(int), UserName = model.UserName };
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

            foreach (Models.Guild x in _context.Guilds)
            {
                model.Guilds.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }


            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            if (_userManager.GetUserId(HttpContext.User) == userId)
            {
                return NotFound();
            }
            else
            {
                User user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(user);
                }
                return RedirectToAction("Users", "Admin");
            }
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
                EditUserViewModel model = new EditUserViewModel()
                {
                    UserId = userId,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    GuildId = user.GuildId ?? default(int),
                    IsAdmin = await _userManager.IsInRoleAsync(user, "admin")
                };


                model.Guilds = new List<SelectListItem>();


                foreach (Models.Guild x in _context.Guilds)
                {
                    SelectListItem xd = new SelectListItem();
                    xd.Text = x.Name;
                    xd.Value = x.Id.ToString();
                    model.Guilds.Add(xd);
                }


                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string userId, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.GuildId = model.GuildId;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        if (model.IsAdmin != await _userManager.IsInRoleAsync(user, "admin"))
                        {
                            if (model.IsAdmin)
                            {
                                await _userManager.AddToRoleAsync(user, "admin");
                            }
                            else
                            {
                                await _userManager.RemoveFromRoleAsync(user, "admin");
                            }
                        }
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
            }
            return View(model);
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


                await _context.AddAsync(guild);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EditGuild(int guildId)
        {
            var guild = _context.Guilds.Where(x => x.Id == guildId).FirstOrDefault();
            var vm = new GuildViewModel()
            {
                Name = guild.Name,
                GuildId = guild.Id
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult EditGuild(GuildViewModel vm)
        {
            var guild = _context.Guilds.Where(x => x.Id == vm.GuildId).FirstOrDefault();
            guild.Name = vm.Name;
            _context.SaveChanges();
            return RedirectToAction("Guilds", "Admin");
        }

        [HttpGet]
        public IActionResult Guilds()
        {
            var guilds = _context.Guilds.Include(x => x.Users).ToList();

            return View(guilds);
        }

        [HttpGet]
        public IActionResult DeleteGuild(int guildId)
        {
            var guild = _context.Guilds.Include(x => x.Users).Where(x => x.Id == guildId).FirstOrDefault();
            try
            {
                if (guild.Users != null)
                {
                    foreach (var x in guild.Users)
                    {
                        x.GuildId = null;
                    }
                }
            }
            catch (Exception e)
            {

            }



            _context.Remove(guild);
            _context.SaveChanges();

            return RedirectToAction("Guilds", "Admin");
        }

        [HttpGet]
        public IActionResult UserOrders(string userId)
        {
            var orders = _context.Orders.Where(x => x.UserId == userId).ToList();

            return View(orders);
        }

        [HttpGet]
        public IActionResult EditUserOrder(int orderId)
        {
            var order = _context.Orders.Where(x => x.Id == orderId).FirstOrDefault();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserOrder(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> EditUserPassword(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            EditUserPasswordViewModel model = new EditUserPasswordViewModel { UserId = user.Id, UserName=user.UserName };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserPassword(EditUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
    }
}