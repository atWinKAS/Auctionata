using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.Services;
using AuctionDemoWebApp.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace AuctionDemoWebApp.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService mailService;
        private IAuctionRepository repository;

        public AppController(IMailService mailService, IAuctionRepository repository)
        {
            this.mailService = mailService;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Items()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError("", "Could not send email due to configuration problems.");
                }

                var sendResult = this.mailService.SendMail(email, model.Email, $"Email from {model.Name} (email: {model.Email})", model.Message);
                if (sendResult)
                {
                    ModelState.Clear();
                    ViewBag.Message = "Mail has been sent.";
                }
            }

            return View();
        }
    }
}
