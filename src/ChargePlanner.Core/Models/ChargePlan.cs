namespace ChargePlanner.Core.Models;

public record ChargePlan(IEnumerable<ChargePeriod> ChargePeriods);