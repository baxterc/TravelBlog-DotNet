﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelBlog.Models;
using Microsoft.AspNetCore.Identity;
using TravelBlog.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TravelBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly TravelBlogDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TravelBlogDbContext db)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index(string str)
        {
            return View(str);
        }
    
        public IActionResult SignUpForm()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignupViewModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                user.AddToMailingList();
                user.MailingList = true;
                return View("Index", "You've been added to the mailing list!");
            }
            else
            {
                return View();
            }
        }
        public IActionResult SignedUp()
        {
            return Content("You've been added to the mailing list!", "text/plain");
        }
    }
}
