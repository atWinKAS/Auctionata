using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

namespace AuctionDemoWebApp.Controllers.Api
{
    [Authorize]
    [Route("api/items")]
    public class ItemController : Controller
    {
        private IAuctionRepository repository;
        private ILogger<ItemController> logger;

        public ItemController(IAuctionRepository repository, ILogger<ItemController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            var items = this.repository.GetUserItemsWithBids(User.Identity.Name);
            var result = Mapper.Map<IEnumerable<ItemViewModel>>(items);
            return Json(result);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]ItemViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newItem = Mapper.Map<Item>(vm);

                    newItem.UserName = User.Identity.Name;
                    newItem.CurrentPrice = 100;

                    this.logger.LogInformation("Going to save data");

                    this.repository.AddItem(newItem);
                    if (this.repository.SaveAll())
                    {
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<ItemViewModel>(newItem));
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Failed to save data", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState });
        }
    }
}
