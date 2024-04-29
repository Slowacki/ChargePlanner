namespace ChargePlanner.Core.Models;

/// <summary>
/// Represents a charge plan
/// </summary>
/// <param name="ChargePeriods">List of charge periods</param>
public record ChargePlan(IEnumerable<ChargePeriod> ChargePeriods);