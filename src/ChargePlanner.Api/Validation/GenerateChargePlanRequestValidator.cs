using ChargePlanner.Api.Endpoints.ChargePlans;
using FluentValidation;

namespace ChargePlanner.Api.Validation;

public class GenerateChargePlanRequestValidator : AbstractValidator<GenerateChargePlanRequest>
{
    public GenerateChargePlanRequestValidator()
    {
        RuleFor(x => x.BatterySettings).NotNull();
        RuleFor(x => x.ChargeSettings).NotNull();
        RuleFor(x => x.BatterySettings.Capacity).GreaterThan(0);
        RuleFor(x => x.BatterySettings.ChargePower).GreaterThan(0);
        RuleFor(x => x.BatterySettings.CurrentLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ChargeSettings.DesiredChargePercentage).GreaterThan(0);
        RuleFor(x => x.ChargeSettings.DesiredChargePercentage).LessThanOrEqualTo(100);
        RuleFor(x => x.ChargeSettings.DirectChargePercentage).GreaterThan(0);
        RuleFor(x => x.ChargeSettings.DirectChargePercentage).LessThanOrEqualTo(100);
        RuleFor(x => x.ChargeSettings.DesiredChargePercentage).GreaterThanOrEqualTo(x => x.ChargeSettings.DirectChargePercentage);
        RuleFor(x => x.ChargeSettings.StartTime).GreaterThan(DateTime.Now);
        RuleFor(x => x.ChargeSettings.EndTime).GreaterThan(DateTime.Now);
        RuleFor(x => x.ChargeSettings.EndTime).GreaterThan(x => x.ChargeSettings.StartTime);
        RuleFor(x => x.ChargeSettings.Tariffs).NotEmpty();
        RuleForEach(x => x.ChargeSettings.Tariffs).SetValidator(new TariffValidator());
    }
}