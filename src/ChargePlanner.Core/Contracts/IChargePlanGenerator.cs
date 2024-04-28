using ChargePlanner.Core.Models;

namespace ChargePlanner.Core.Contracts;

public interface IChargePlanGenerator
{
    Task<ChargePlan> GenerateAsync(ChargeSettings chargeSettings, BatterySettings batterySettings, CancellationToken cancellationToken);
}