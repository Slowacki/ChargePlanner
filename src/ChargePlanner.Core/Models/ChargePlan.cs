namespace ChargePlanner.Core.Models;

public record ChargePlan(IEnumerable<ChargingPeriod> ChargePeriods);