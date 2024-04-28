using System.Net;
using Asp.Versioning;
using ChargePlanner.Core.Contracts;
using ChargePlanner.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChargePlanner.Api.Endpoints.ChargePlans;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/charge-plans/generate")]
[Produces("application/json")]
public class Generate(IChargePlanGenerator chargePlanGenerator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> Execute(CancellationToken cancellationToken)
    {
        var tariffs = new List<Tariff>
        {
            new(new TimeOnly(0, 0), new TimeOnly(12, 30), 1),
            new(new TimeOnly(12, 30), new TimeOnly(23, 59), 2)
        };

        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddHours(-10);
        var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddHours(2);
        var chargeSettings = new ChargeSettings(100, start, end, 25, tariffs);
        var batterySettings = new BatterySettings(20, 200, 100);

        var chargePlan =
            await chargePlanGenerator.GenerateAsync(chargeSettings, batterySettings, cancellationToken);

        return Ok(chargePlan);
    }
}