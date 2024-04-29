using Asp.Versioning;
using ChargePlanner.Api.Endpoints.ChargePlans;
using ChargePlanner.Api.Validation;
using ChargePlanner.Core.Contracts;
using FluentValidation;

namespace ChargePlanner.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        
        // Add services to the container.
        services.AddScoped<IChargePlanGenerator, ChargePlanGenerator>();
        services.AddScoped<IValidator<GenerateChargePlanRequest>, GenerateChargePlanRequestValidator>();
        
        services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddDateOnlyTimeOnlyStringConverters();
        services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection()
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        app.Run();
    }
}