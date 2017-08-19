using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using blackbelt.Models;
using blackbelt.Factory;

namespace blackbelt.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserFactory userFactory;
        public HomeController(UserFactory connection)
        {
            userFactory = connection;
        }

        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("validate")]
        public IActionResult Validate(User newUser)
        {
            if(ModelState.IsValid)
            {
               List<User> testing = userFactory.TestUser(newUser);
               if(testing.Count > 0)
               {
                   ViewBag.error = "Username taken";
                   return View("Index");
               }
               else
               {
                   userFactory.CreateUser(newUser);
                   return RedirectToAction("Index");
               }
            }
            else
            {
                System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return View("Index");

            }
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View("login");
        }

        [HttpPost]
        [Route("try")]
        public IActionResult Log(UserTest test)
        {
            TryValidateModel(test);
            if(ModelState.IsValid)
            {
                
                User grab = userFactory.LoginUser(test);
                if(grab != null)
                {
                    HttpContext.Session.SetInt32("logged", grab.Id);
                    return RedirectToAction("Home", "LandingPage");
                }
                else
                {
                    ViewBag.error = "Invalid username/password";
                    return View("login");
                }   
            }
            else
            {
                ViewBag.error = "Invalid username/password";
                return View("Login");
            }
        }
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return View("login");
        }

    }
}

