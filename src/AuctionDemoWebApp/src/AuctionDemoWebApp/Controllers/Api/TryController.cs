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
    [Route("api/items/{itemName}/try")]
    public class TryController : Controller
    {
        private ILogger<TryController> logger;
        private IAuctionRepository repository;
        private CalculationService calculationService;
        private IHubContext commHub;

        public TryController(IAuctionRepository repository, ILogger<TryController> logger, CalculationService calculationService, IConnectionManager connectionManager)
        {
            this.repository = repository;
            this.logger = logger;
            this.calculationService = calculationService;

            this.commHub = connectionManager.GetHubContext<AuctionDemoWebApp.Hub.CommunicationHub>();
        }

        [HttpPost("")]
        public JsonResult Post(string itemName, [FromBody] TryBidViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = new TryBidResponse()
                    {
                        ItemName = vm.ItemName,
                        IsHighest = false
                    };

                    //// saving this bid
                    var newBid = new Bid()
                    {
                        Created = DateTime.Now,
                        UserName = User.Identity.Name,
                        Price = vm.ClientPrice
                    };
                    this.repository.AddBid(itemName, newBid);

                    if (this.repository.SaveAll())
                    {
                        var item = this.repository.GetItemByName(itemName);
                        if (item.Bids.Count == 1)
                        {
                            result.IsHighest = true;
                            result.Price = newBid.Price;
                            result.Created = newBid.Created;
                            result.UserName = newBid.UserName;
                        }
                        else
                        {
                            //// here i need to check if i have first bid
                            var firstBib = item.Bids.OrderBy(b => b.Created).FirstOrDefault();
                            if (firstBib.UserName.Equals(User.Identity.Name))
                            {
                                //// first bid was placed by me
                                result.IsHighest = true;
                                result.Price = firstBib.Price;
                                result.Created = firstBib.Created;
                                result.UserName = firstBib.UserName;
                            }
                            else
                            {
                                //// first bid was placed by other user
                                result.IsHighest = false;
                                result.Price = firstBib.Price;
                                result.Created = firstBib.Created;
                                result.UserName = firstBib.UserName;
                            }
                        }
                    }

                    this.NotifyAll(User.Identity.Name, vm.ItemName, vm.ClientPrice);
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(result);
                }

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "Validation errors while saving details. Errors: ", ModelState = ModelState });
            }
            catch (Exception ex)
            {
                this.logger.LogError("Failed to try to bid ", ex);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json("Failed to try to bid");
            }
        }

        private void NotifyAll(string userName, string itemName, double newValue)
        {
            this.logger.LogInformation($"Going to notify {UserHandler.ConnectedIds.Count} clients...");

            this.commHub.Clients.All.priceChanged(userName, itemName, newValue);
        }
    }
}
