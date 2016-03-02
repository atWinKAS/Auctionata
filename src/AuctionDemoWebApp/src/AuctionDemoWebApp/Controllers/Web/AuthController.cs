using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;

namespace AuctionDemoWebApp.Controllers.Web
{
    public class AuthController : Controller
    {
        private SignInManager<AuctionUser> signInManager;

        public AuthController(SignInManager<AuctionUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Items", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(vm.Username, vm.Password, true, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Items", "App");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                }
            }

            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await this.signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "App");
        }
    }
}
