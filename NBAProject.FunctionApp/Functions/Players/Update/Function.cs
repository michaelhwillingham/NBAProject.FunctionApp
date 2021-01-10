using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace NBAProject.FunctionApp.Functions.Players.Update
{
    public class Function
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public Function(IMediator mediator, ILogger<Function> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [FunctionName("UpdatePlayersData")]
        public async Task RunAsync([TimerTrigger("0 */10 * * * *")] TimerInfo timerInfo)
        {
            _logger.LogInformation($"Function executed at: {DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)}");

            await _mediator.Send(new Request {Season = "current"});
        }
    }
}