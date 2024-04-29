using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using ChargePlanner.Api.Endpoints.ChargePlans;
using ChargePlanner.Core.Models;

namespace ChargePlanner.Api.Tests.Endpoints.ChargePlans;

// Example tests
[Collection(ComponentTestsCollection.Name)]
public class CreateTests : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Fixture _fixture = new();

    public CreateTests(WebAppFactory webAppFactory)
    {
        _httpClient = webAppFactory.CreateClient();
    }
    
    [Fact(DisplayName = "Execute returns a plan that immediately starts charging if direct charge below current level")]
    public async Task Execute_ReturnsDirectCharging_WhenDirectChargeBelowCurrentLevel()
    {
        var directChargingPeriodPrice = 2;
        var tarrifs = new List<Tariff>()
        {
            new Tariff(new TimeOnly(0, 0), new TimeOnly(7, 30), 1),
            new Tariff(new TimeOnly(7, 30), new TimeOnly(18, 30), 3),
            new Tariff(new TimeOnly(18, 30), new TimeOnly(23, 59, 59), directChargingPeriodPrice)
        };
        
        var batterySettings = new BatterySettings(10, 100, 0);
        var chargeSettings = new ChargeSettings(50, 
            new DateTime(2345, 06, 01, 18, 0, 0), 
            new DateTime(2345, 06, 02, 2, 0, 0), 
            10,
            tarrifs);

        var request = new GenerateChargePlanRequest(batterySettings, chargeSettings);

        using var response = await _httpClient.PostAsJsonAsync($"v1/charge-plans/generate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var planResponse = await response.Content.ReadFromJsonAsync<ChargePlanResponse>();

        var firstChargingPeriod = planResponse!.ChargePeriods.First();
        
        Assert.Equal(directChargingPeriodPrice, firstChargingPeriod.ChargingPricePerKwh);
        Assert.True(firstChargingPeriod.IsCharging);
    }
    
    [Fact(DisplayName = "Execute returns a plan with all charging periods set to true if total charge time is less than required for desired charge level")]
    public async Task Execute_ReturnsPlanWithAllPeriodsSetToTrue_WhenTotalChargeTimeIsLessThanNeededForDesiredCharge()
    {
        // throw new NotImplementedException();
    }
    
    // TODO: More tests for the logic

    [Fact(DisplayName = "Execute returns bad request when charge power is zero")]
    public async Task Execute_ReturnsBadRequest_WhenChargePowerIsZero()
    {
        var tarrifs = new List<Tariff>()
        {
            new Tariff(new TimeOnly(0, 0), new TimeOnly(7, 30), 1),
            new Tariff(new TimeOnly(7, 30), new TimeOnly(18, 30), 2),
            new Tariff(new TimeOnly(18, 30), new TimeOnly(23, 59, 59), 3)
        };
        
        var batterySettings = new BatterySettings(0, 100, 10);
        var chargeSettings = new ChargeSettings(50, DateTime.Now.AddHours(5), DateTime.Now.AddHours(15), 10,
            tarrifs);

        var request = new GenerateChargePlanRequest(batterySettings, chargeSettings);

        using var response = await _httpClient.PostAsJsonAsync($"v1/charge-plans/generate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        //var responseString = await response.Content.ReadAsStringAsync();
        //TODO: Check for specific message
    }
    
    [Fact(DisplayName = "Execute returns bad request when battery capacity is less than or equal to 0")]
    public async Task Execute_ReturnsBadRequest_WhenBatteryCapacityIsLessOrEqualToZero()
    {
        //throw new NotImplementedException();
    }
    
    // TODO: More tests for the validation rules

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}