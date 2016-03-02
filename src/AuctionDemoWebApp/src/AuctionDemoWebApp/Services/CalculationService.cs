using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AuctionDemoWebApp.Services
{
    public class CalculationService
    {
        private ILogger<CalculationService> logger;

        public CalculationService(ILogger<CalculationService> logger)
        {
            this.logger = logger;
        }

        public CalculationResult Calculate(double currentPrice)
        {
            // dummy new price calculation routine
            // for a now just add 1 to the current price

            var result = new CalculationResult
            {
                Success = true,
                NewPrice = currentPrice + 10
            };

            return result;
        }
    }
}
