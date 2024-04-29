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
    public async Task<ActionResult> Execute([FromBody] GenerateChargePlan command, CancellationToken cancellationToken)
    {
        // var tariffs = new List<Tariff>
        // {
        //     new(new TimeOnly(0, 0), new TimeOnly(12, 30), 1),
        //     new(new TimeOnly(12, 30), new TimeOnly(23, 59), 2)
        // };
        //
        // var now = DateTime.Now;
        // var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddHours(-10);
        // var end = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddHours(6);
        // var chargeSettings = new ChargeSettings(100, start, end, 25, tariffs);
        // var batterySettings = new BatterySettings(20, 200, 100);

        var chargePlan =
            await chargePlanGenerator.GenerateAsync(command.ChargeSettings, command.BatterySettings, cancellationToken);
        
        return Ok(MapToResponse(chargePlan));
    }

    private ChargePlanResponse MapToResponse(ChargePlan chargePlan)
    {
        var chargePlanResponse = new ChargePlanResponse();

        foreach (var chargePeriod in chargePlan.ChargePeriods)
        {
            if (chargePeriod.ChargeLength.TotalMinutes == 0)
            {
                chargePlanResponse.ChargePeriods.Add(new()
                {
                    StartTime = chargePeriod.StartTime,
                    EndTime = chargePeriod.EndTime,
                    ChargingPricePerKwh = chargePeriod.ChargingPricePerKwh
                });
            }
            else if (chargePeriod.IdleLength.TotalMinutes == 0)
            {
                chargePlanResponse.ChargePeriods.Add(new()
                {
                    StartTime = chargePeriod.StartTime,
                    EndTime = chargePeriod.EndTime,
                    IsCharging = true,
                    ChargingPricePerKwh = chargePeriod.ChargingPricePerKwh
                });
            }
            else
            {
                chargePlanResponse.ChargePeriods.Add(new()
                {
                    StartTime = chargePeriod.StartTime,
                    EndTime = chargePeriod.StartTime.AddMinutes(chargePeriod.ChargeLength.TotalMinutes),
                    IsCharging = true,
                    ChargingPricePerKwh = chargePeriod.ChargingPricePerKwh
                });
                chargePlanResponse.ChargePeriods.Add(new()
                {
                    StartTime = chargePeriod.StartTime.AddMinutes(chargePeriod.ChargeLength.TotalMinutes),
                    EndTime = chargePeriod.EndTime,
                    IsCharging = false,
                    ChargingPricePerKwh = chargePeriod.ChargingPricePerKwh
                });
            }
        }

        return chargePlanResponse;
    }
    
    private class ChargePlanResponse
    {
        public List<ChargePeriodResponse> ChargePeriods { get; set; } = new();
    }

    private class ChargePeriodResponse
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCharging { get; set; }
        public decimal ChargingPricePerKwh { get; set; }
    }

    public class GenerateChargePlan
    {
        public BatterySettings BatterySettings { get; set; } = null!;
        public ChargeSettings ChargeSettings { get; set; } = null!;
    }
}