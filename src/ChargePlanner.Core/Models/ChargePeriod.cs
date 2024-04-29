namespace ChargePlanner.Core.Models;

public class ChargePeriod(DateTime startTime, DateTime endTime, decimal chargePricePerKwh)
{
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public Decimal ChargePricePerKwh { get; set; } = chargePricePerKwh;
    public TimeSpan ChargeLength { get; set; } = TimeSpan.Zero;
    public TimeSpan Length => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime).TotalMinutes));
    public TimeSpan IdleLength => TimeSpan.FromMinutes(Math.Round((EndTime - StartTime - ChargeLength).TotalMinutes));
}