using ChargePlanner.Core.Models;
using FluentValidation;

namespace ChargePlanner.Api.Validation;

public class TariffValidator : AbstractValidator<Tariff>
{
    public TariffValidator()
    {
        RuleFor(x => x.PricePerKwh).GreaterThan(0);

    }
}