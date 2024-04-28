﻿using ChargePlanner.Core.Models;

namespace ChargePlanner.Core.Contracts;

public class ChargePlanGenerator : IChargePlanGenerator
{
    public async Task<ChargePlan> GenerateAsync(ChargeSettings chargeSettings,
        BatterySettings batterySettings,
        CancellationToken cancellationToken)
    {
        var chargingStartTime = chargeSettings.StartTime;
        var chargingEndTime = chargeSettings.EndTime;
        var timeForCharging = chargingEndTime - chargingStartTime;
        var timeToDesiredCharge = batterySettings.GetRemainingChargeTime(chargeSettings.DesiredChargePercentage);
        var timeToDirectCharge = batterySettings.GetRemainingChargeTime(chargeSettings.DirectChargePercentage);

        var chargePeriods = GenerateChargingPeriods(chargingStartTime, chargingEndTime, chargeSettings.Tariffs);

        if (timeToDesiredCharge > timeForCharging || timeToDirectCharge > timeForCharging)
        {
            chargePeriods.ForEach(c => c.ChargeLength = c.Length);

            return new ChargePlan(chargePeriods);
        }

        var i = 0;
        
        while (timeToDirectCharge.TotalSeconds > 0)
        {
            var remainingPeriodLength = chargePeriods[i].IdleLength;

            chargePeriods[i].ChargeLength = timeToDirectCharge > remainingPeriodLength ? remainingPeriodLength : timeToDirectCharge;
            
            timeToDesiredCharge -= remainingPeriodLength;
            timeToDirectCharge -= remainingPeriodLength;
            i++;
        }

        var costOrderedPeriods = chargePeriods.OrderBy(c => c.ChargingPricePerKwh).ToList();
        var j = 0;

        while (timeToDesiredCharge.TotalSeconds > 0)
        {
            var remainingPeriodLength = costOrderedPeriods[j].IdleLength;

            costOrderedPeriods[j].ChargeLength = timeToDesiredCharge > remainingPeriodLength ? remainingPeriodLength : timeToDesiredCharge;
            
            timeToDesiredCharge -= costOrderedPeriods[j].ChargeLength;
            j++;
        }
        
        return new ChargePlan(costOrderedPeriods.OrderBy(c => c.StartTime));
    }

    private List<ChargingPeriod> GenerateChargingPeriods(
        DateTime startTime, 
        DateTime endTime, 
        IEnumerable<Tariff> tariffs)
    {
        tariffs = tariffs.ToList();
        var remainingChargingTime = (endTime - startTime).TotalMinutes;
        var chargePeriods = new List<ChargingPeriod>();

        while (remainingChargingTime > 0)
        {
            var currentTariff = tariffs.Single(t =>
                startTime.TimeOfDay >= t.StartTime.ToTimeSpan() && startTime.TimeOfDay < t.EndTime.ToTimeSpan());

            var tariffEndTime = new DateTime(DateOnly.FromDateTime(startTime), currentTariff.EndTime, startTime.Kind);
            
            var chargePeriod = new ChargingPeriod(startTime,
                endTime < tariffEndTime ? endTime : tariffEndTime,
                currentTariff.PricePerKwh);
            
            chargePeriods.Add(chargePeriod);

            var periodLength = Math.Round(chargePeriod.Length.TotalMinutes, MidpointRounding.ToZero) + 1;

            remainingChargingTime -= periodLength;

            startTime = startTime.AddMinutes(periodLength);
        }

        return chargePeriods;
    }
}