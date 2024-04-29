using ChargePlanner.Core.Models;

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
        var timeToDirectCharge = batterySettings.GetRemainingChargeTime(chargeSettings.DirectChargePercentage);
        var remainingTimeToDesiredCharge = batterySettings.GetRemainingChargeTime(chargeSettings.DesiredChargePercentage) - timeToDirectCharge;

        var chargePeriods = GenerateChargingPeriods(chargingStartTime, chargingEndTime, chargeSettings.Tariffs);

        if (timeToDirectCharge > timeForCharging || remainingTimeToDesiredCharge > timeForCharging)
        {
            chargePeriods.ForEach(c => c.ChargeLength = c.Length);

            return new ChargePlan(chargePeriods);
        }

        chargePeriods = ApplyChargingTime(timeToDirectCharge, chargePeriods);

        chargePeriods = chargePeriods.OrderBy(c => c.ChargePricePerKwh).ToList();
        
        chargePeriods = ApplyChargingTime(remainingTimeToDesiredCharge, chargePeriods);
        
        return new ChargePlan(chargePeriods.OrderBy(c => c.StartTime));
    }

    private List<ChargePeriod> GenerateChargingPeriods(
        DateTime startTime, 
        DateTime endTime, 
        IEnumerable<Tariff> tariffs)
    {
        tariffs = tariffs.ToList();
        var remainingChargingTime = (endTime - startTime).TotalMinutes;
        var chargePeriods = new List<ChargePeriod>();

        while (remainingChargingTime > 0)
        {
            var currentTariff = tariffs.Single(t =>
                startTime.TimeOfDay >= t.StartTime.ToTimeSpan() && startTime.TimeOfDay < t.EndTime.ToTimeSpan());

            var tariffEndTime = new DateTime(DateOnly.FromDateTime(startTime), currentTariff.EndTime, startTime.Kind);
            
            var chargePeriod = new ChargePeriod(startTime,
                endTime < tariffEndTime ? endTime : tariffEndTime,
                currentTariff.PricePerKwh);
            
            chargePeriods.Add(chargePeriod);

            var periodLength = Math.Round(chargePeriod.Length.TotalMinutes, MidpointRounding.ToEven);

            remainingChargingTime -= periodLength;

            startTime = startTime.AddMinutes(periodLength);
        }

        return chargePeriods;
    }

    private List<ChargePeriod> ApplyChargingTime(TimeSpan chargingTime, List<ChargePeriod> orderedChargePeriods)
    {
        var i = 0;
        
        while (chargingTime.TotalMinutes > 0)
        {
            var remainingPeriodLength = orderedChargePeriods[i].IdleLength;

            var chargeLength = chargingTime > remainingPeriodLength ? remainingPeriodLength : chargingTime;

            orderedChargePeriods[i].ChargeLength += chargeLength;
            
            chargingTime -= chargeLength;
            i++;
        }

        return orderedChargePeriods;
    }
}