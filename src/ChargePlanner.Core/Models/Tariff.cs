namespace ChargePlanner.Core.Models;

public record Tariff(TimeOnly StartTime, TimeOnly EndTime, Decimal PricePerKwh);