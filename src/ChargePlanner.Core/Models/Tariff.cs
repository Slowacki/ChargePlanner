namespace ChargePlanner.Core.Models;

// TODO: Rewrite without EndTime
/// <summary>
/// Represents the electricity tariff
/// </summary>
/// <param name="StartTime">Start time of the tariff</param>
/// <param name="EndTime">End time of the tariff</param>
/// <param name="PricePerKwh">The price in € per kWh</param>
public record Tariff(TimeOnly StartTime, TimeOnly EndTime, Decimal PricePerKwh);