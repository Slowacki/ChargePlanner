using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ChargePlanner.Api.Tests;

public sealed class WebAppFactory : WebApplicationFactory<Program>
{
    public WebAppFactory()
    {
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
    }
}