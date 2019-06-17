using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class LoginController : Controller
    {
        private MyContext dbContext;
        public LoginController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(i => i.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Register");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                
                var thisuser = dbContext.Users.FirstOrDefault(i => i.Email == user.Email);
                HttpContext.Session.SetInt32("UserID", thisuser.Id);
                int? IntVariable = HttpContext.Session.GetInt32("UserID");
                System.Console.WriteLine(IntVariable);
                return RedirectToAction("Index", "Home");
            }
            return View("Register");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("loginuser")]
        public IActionResult LoginUser(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var thisuser = dbContext.Users.FirstOrDefault(i => i.Email == user.Email);
                if(thisuser == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, thisuser.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("UserID", thisuser.Id);
                int? IntVariable = HttpContext.Session.GetInt32("UserID");
                System.Console.WriteLine(IntVariable);
                // int sessionID = IntVariable ?? default(int);
                return RedirectToAction("Index", "Home"); 
            }
            System.Console.WriteLine("Model not valid");
            return View("Login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}


// if (HttpContext.Session.GetInt32("UserID") == null)
// {
//     return RedirectToAction("Register", "Login");
// }