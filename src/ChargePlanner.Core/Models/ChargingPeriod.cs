namespace ChargePlanner.Core.Models;

public class ChargingPeriod(DateTime startTime, DateTime endTime, decimal chargingPricePerKwh)
{
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public Decimal ChargingPricePerKwh { get; set; } = chargingPricePerKwh;
    public TimeSpan ChargeLength { get; set; } = TimeSpan.Zero;
    public TimeSpan Length => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime).TotalMinutes));
    public TimeSpan IdleLength => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime - ChargeLength).TotalMinutes));
}