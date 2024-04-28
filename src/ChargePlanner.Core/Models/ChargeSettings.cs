namespace ChargePlanner.Core.Models;

public record ChargeSettings(int DesiredChargePercentage, 
    DateTime StartTime, 
    DateTime EndTime, 
    int DirectChargePercentage, 
    IEnumerable<Tariff> Tariffs);