using System.Net;
using Asp.Versioning;
using ChargePlanner.Core.Contracts;
using ChargePlanner.Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ChargePlanner.Api.Endpoints.ChargePlans;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/charge-plans/generate")]
[Produces("application/json")]
public class Generate(IChargePlanGenerator chargePlanGenerator, IValidator<GenerateChargePlanRequest> requestValidator) : ControllerBase
{
    /// <summary>
    /// Generates and returns a charge plan for the given battery and charging settings
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Charge plan specifying in which hours should the battery be charged and the related tariff price</returns>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Execute([FromBody] GenerateChargePlanRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);

        if(!validationResult.IsValid)
        {
            return BadRequest(validationResult);
        }
        
        var chargePlan =
            await chargePlanGenerator.GenerateAsync(request.ChargeSettings, request.BatterySettings, cancellationToken);
        
        return Ok(MapToResponse(chargePlan));
    }

    private ChargePlanResponse MapToResponse(ChargePlan chargePlan)
    {
        var chargePeriods = new List<ChargePeriodResponse>();

        foreach (var chargePeriod in chargePlan.ChargePeriods)
        {
            if (chargePeriod.ChargeLength.TotalMinutes == 0)
            {
                chargePeriods.Add(new(chargePeriod.StartTime, 
                    chargePeriod.EndTime, 
                    false, 
                    chargePeriod.ChargingPricePerKwh));
            }
            else if (chargePeriod.IdleLength.TotalMinutes == 0)
            {
                chargePeriods.Add(new(chargePeriod.StartTime, 
                    chargePeriod.EndTime, 
                    true, 
                    chargePeriod.ChargingPricePerKwh));
            }
            else
            {
                chargePeriods.Add(new(chargePeriod.StartTime, 
                    chargePeriod.StartTime.AddMinutes(chargePeriod.ChargeLength.TotalMinutes), 
                    true, 
                    chargePeriod.ChargingPricePerKwh));
                chargePeriods.Add(new(chargePeriod.StartTime.AddMinutes(chargePeriod.ChargeLength.TotalMinutes), 
                    chargePeriod.EndTime, 
                    false, 
                    chargePeriod.ChargingPricePerKwh));
            }
        }

        return new ChargePlanResponse(chargePeriods);
    }
}

public record ChargePlanResponse(List<ChargePeriodResponse> ChargePeriods);

public record ChargePeriodResponse(DateTime StartTime, DateTime EndTime, bool IsCharging, decimal ChargingPricePerKwh);

public record GenerateChargePlanRequest(BatterySettings BatterySettings, ChargeSettings ChargeSettings);