using System.Data;
using Api;
using Api.Mapping;
using Api.Seed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    options.UseInMemoryDatabase("StoreDb");
});
var resource = ResourceBuilder.CreateDefault()
    .AddService(serviceName: "App", serviceVersion: "1.0.0");
// builder.Services.AddScoped<IDbConnection>(sp =>
//     new SqlConnection(builder.Configuration.GetConnectionString("StoreDb")));

builder.Services.AddHostedService<DataSeedingService>();

//OpenTelemetry configurations
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "App", serviceVersion: "1.0.0"))
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .SetResourceBuilder(resource)
            .AddAspNetCoreInstrumentation() // Instruments incoming ASP.NET Core requests
            .AddHttpClientInstrumentation(); // Instruments outgoing HTTP client requests
    })
    .WithLogging()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .SetResourceBuilder(resource)
            .AddAspNetCoreInstrumentation() // Instruments ASP.NET Core metrics
            .AddHttpClientInstrumentation() // Instruments HTTP client metrics
            .AddRuntimeInstrumentation(); // Instruments .NET runtime metrics
    }).UseOtlpExporter();



builder.Logging.AddOpenTelemetry(opt =>
{
    opt.SetResourceBuilder(resource);
    opt.IncludeFormattedMessage = true;
    opt.IncludeScopes = true;
});

var app = builder.Build();
{
    app.UseHttpsRedirection();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapOpenApi();
    }

    app.MapControllers();
    app.MapGet("/live", () => "I am alive !");
}
app.Run();