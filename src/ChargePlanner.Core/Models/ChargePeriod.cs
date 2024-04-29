namespace ChargePlanner.Core.Models;

/// <summary>
/// Represents a potential charging period
/// </summary>
/// <param name="startTime">Start time</param>
/// <param name="endTime">End Time</param>
/// <param name="chargePricePerKwh">Price in € per kWh</param>
public class ChargePeriod(DateTime startTime, DateTime endTime, decimal chargePricePerKwh)
{
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public Decimal ChargePricePerKwh { get; set; } = chargePricePerKwh;
    public TimeSpan ChargeLength { get; set; } = TimeSpan.Zero;
    public TimeSpan Length => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime).TotalMinutes));
    public TimeSpan IdleLength => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime - ChargeLength).TotalMinutes));
}