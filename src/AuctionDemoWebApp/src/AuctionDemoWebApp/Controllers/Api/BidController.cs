using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionDemoWebApp.Hub;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.Services;
using AuctionDemoWebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AuctionDemoWebApp.Controllers.Api
{
    [Microsoft.AspNet.Authorization.Authorize]
    [Route("api/items/{itemName}/bids")]
    public class BidController : Controller
    {
        private ILogger<BidController> logger;
        private IAuctionRepository repository;
        private CalculationService calculationService;
        private IHubContext commHub;

        public BidController(IAuctionRepository repository, ILogger<BidController> logger, CalculationService calculationService, IConnectionManager connectionManager)
        {
            this.repository = repository;
            this.logger = logger;
            this.calculationService = calculationService;

            this.commHub = connectionManager.GetHubContext<AuctionDemoWebApp.Hub.CommunicationHub>();
        }

        [HttpGet("")]
        public JsonResult Get(string itemName)
        {
            try
            {
                var result = this.repository.GetItemByName(itemName);
                if (result == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<BidViewModel>>(result.Bids.OrderByDescending(s => s.Created)));

            }
            catch (Exception ex)
            {
                this.logger.LogError($"Unable to get details for {itemName}", ex);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Error while getting details");
            }
        }

        [HttpPost("")]
        public JsonResult Post(string itemName, [FromBody] BidViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newBid = Mapper.Map<Bid>(vm);
                    
                    newBid.Created = DateTime.UtcNow;
                    newBid.UserName = User.Identity.Name;

                    this.logger.LogInformation($"Saving bid: { Newtonsoft.Json.JsonConvert.SerializeObject(newBid) }");

                    this.repository.AddBid(itemName, newBid);

                    if (this.repository.SaveAll())
                    {
                        //var item = this.repository.GetItemByName(itemName);
                        //double currentPrice = 0;
                        //if (item.Bids.Any())
                        //{
                        //    var latestBib = item.Bids.OrderByDescending(b => b.Created).FirstOrDefault();
                        //    if (latestBib != null)
                        //    {
                        //        currentPrice = latestBib.Price;
                        //    }
                        //}

                        //var calcResult = this.calculationService.Calculate(currentPrice);
                        //if (calcResult.Success)
                        //{
                        //    this.NotifyOthers(item.Name, calcResult.NewPrice);
                        //    this.Log($"Current new price is {calcResult.NewPrice}");
                        //}

                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<BidViewModel>(newBid));
                    }
                }

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new {Message = "Validation errors while saving details. Errors: " , ModelState = ModelState });
            }
            catch (Exception ex)
            {
                this.logger.LogError("Failed to save details", ex);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Failed to save details");
            }
        }

        private void NotifyOthers(string itemName, double newValue)
        {
            this.logger.LogInformation($"Going to notify {UserHandler.ConnectedIds.Count} clients..."); 

            this.commHub.Clients.All.priceChanged(itemName, newValue);

        }
    }
}
