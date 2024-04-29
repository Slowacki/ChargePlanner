namespace ChargePlanner.Core.Models;

/// <summary>
/// Represents the settings of the charging to be done
/// </summary>
/// <param name="DesiredChargePercentage">Percentage of capacity the battery should be charged to</param>
/// <param name="StartTime">The earliest start time of the charging process</param>
/// <param name="EndTime">The time when the charging has to stop</param>
/// <param name="DirectChargePercentage">Percentage of battery capacity that always needs to charged immediately</param>
/// <param name="Tariffs">Energy tariffs</param>
public record ChargeSettings(int DesiredChargePercentage, 
    DateTime StartTime, 
    DateTime EndTime, 
    int DirectChargePercentage, 
    IEnumerable<Tariff> Tariffs);