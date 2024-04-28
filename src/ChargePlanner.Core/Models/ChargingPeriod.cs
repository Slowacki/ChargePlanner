namespace ChargePlanner.Core.Models;

public class ChargingPeriod(DateTime startTime, DateTime endTime, decimal chargingPricePerKwh)
{
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;
    public Decimal ChargingPricePerKwh { get; set; } = chargingPricePerKwh;
    public TimeSpan ChargeLength { get; set; } = TimeSpan.Zero;
    public TimeSpan Length => EndTime - StartTime;
    public TimeSpan IdleLength => EndTime - StartTime - ChargeLength;
}