using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guild.Data;
using Guild.Models;
using Guild.UI.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Guild.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GuildContext _context;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, GuildContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var kkms = _context.Orders.Where(x => x.UserId == _userManager.GetUserId(HttpContext.User)).ToList();

            return View(kkms);
        }
        
        [HttpGet]
        public IActionResult Details(int orderId)
        {
            var result = _context.Orders.Where(x => x.Id == orderId && x.UserId == _userManager.GetUserId(HttpContext.User)).FirstOrDefault();

            if (result != null)
            {
                return View(result);
            }
            else
            {
                return NotFound();
            }            
        }
    }
}