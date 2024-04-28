namespace ChargePlanner.Core.Models;

public record BatterySettings(decimal ChargePower, decimal Capacity, decimal CurrentLevel)
{
    public TimeSpan GetRemainingChargeTime(int desiredChargePercentage = 100)
    {
        var toCharge = ((decimal)desiredChargePercentage / 100) * Capacity - CurrentLevel;

        var remainingTimeInHours = toCharge / ChargePower;
        
        return TimeSpan.FromHours((double)remainingTimeInHours);
    }
}