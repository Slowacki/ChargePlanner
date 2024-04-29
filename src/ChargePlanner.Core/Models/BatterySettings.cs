namespace ChargePlanner.Core.Models;

/// <summary>
/// Represents the parameters of the battery to be charged
/// </summary>
/// <param name="ChargePower">Charge power that can be handled by the battery in kW</param>
/// <param name="Capacity">Maximum capacity of the battery in kWh</param>
/// <param name="CurrentLevel">Current charge level in kWh</param>
public record BatterySettings(decimal ChargePower, decimal Capacity, decimal CurrentLevel)
{
    public TimeSpan GetRemainingChargeTime(int desiredChargePercentage = 100)
    {
        var toCharge = ((decimal)desiredChargePercentage / 100) * Capacity - CurrentLevel;

        var remainingTimeInHours = toCharge / ChargePower;
        
        return TimeSpan.FromHours((double)remainingTimeInHours);
    }
}