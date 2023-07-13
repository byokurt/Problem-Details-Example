using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProblemDetailsExample.Constant;
using ProblemDetailsExample.Data;
using ProblemDetailsExample.Data.Seeds;
using ProblemDetailsExample.Extensions;
using ProblemDetailsExample.Filters;
using ProblemDetailsExample.Middleware;
using ProblemDetailsExample.Proxies.Demo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options => { options.ReportApiVersions = true; });

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = AppConstant.AppVersion;
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers(options => { options.Filters.Add(new ProblemDetailsExceptionFilter()); }).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHeaderPropagation(o =>
{
    o.Headers.Add("ClientId", "DemoClientId");
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>().AddFluentValidationAutoValidation(fv => fv.DisableDataAnnotationsValidation = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger();

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "Liveness", "Readiness" });

builder.Services.AddDbContext<DemoDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DemoCnn") ?? string.Empty, sqlOptions => { sqlOptions.EnableRetryOnFailure(3); }); });

builder.Services.AddHttpClient<IDemoApiProxy, DemoApiProxy>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["DemoApiUrl"] ?? string.Empty);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddPolicyHandler(HttpPolicies.GetRetryPolicy).AddPolicyHandler(HttpPolicies.GetCircuitBreakerPolicy());

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddMasTransit(builder.Configuration);

builder.Services.AddBackgroundService();

var app = builder.Build();

await Seeder.MigrateWithData(app);

app.UsePathBase("/demo");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/demo/swagger/v1/swagger.json", "Demo API v1");
    });
}

app.UseMiddleware<LoggingMiddleware>();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/live");

app.MapHealthChecks("/ready");

app.Run();