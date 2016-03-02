using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.Services;
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
        private CalculationService calculationService;

        public ItemController(IAuctionRepository repository, ILogger<ItemController> logger, CalculationService calculationService)
        {
            this.repository = repository;
            this.logger = logger;
            this.calculationService = calculationService;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            var items = this.repository.GetItemsWithBids();
            var result = Mapper.Map<IEnumerable<ItemViewModel>>(items);

            foreach (var ri in result)
            {
                ri.CurrentPrice = 0;
                var realItem = items.FirstOrDefault(i => i.Id == ri.Id);
                if (realItem != null && realItem.Bids != null && realItem.Bids.Any())
                {
                    var latestBib = realItem.Bids.OrderByDescending(b => b.Created).FirstOrDefault();
                    if (latestBib != null)
                    {
                        ri.CurrentPrice = latestBib.Price;
                    }
                }
            }

            return Json(result);
        }

        [HttpGet("{itemName}")]
        public JsonResult Get(string itemName)
        {
            var item = this.repository.GetItemByName(itemName);
            var result = Mapper.Map<ItemViewModel>(item);

            double currentPrice = 0;
            if (item.Bids.Any())
            {
                var latestBib = item.Bids.OrderByDescending(b => b.Created).FirstOrDefault();
                if (latestBib != null)
                {
                    currentPrice = latestBib.Price;
                }
            }

            var calcResult = this.calculationService.Calculate(currentPrice);
            if (calcResult.Success)
            {
                result.CurrentPrice = calcResult.NewPrice;
            }

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
