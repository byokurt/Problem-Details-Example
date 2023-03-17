using System.Net.Http.Headers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProblemDetailsExample.Constant;
using ProblemDetailsExample.Data;
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

builder.Services.AddControllers(options => { options.Filters.Add(new ProblemDetailsExceptionFilter()); });

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "Liveness", "Readiness" });

builder.Services.AddDbContext<DemoDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DemoCnn") ?? string.Empty, sqlOptions => { sqlOptions.EnableRetryOnFailure(3); }); });

builder.Services.AddHttpClient<IDemoApiProxy, DemoApiProxy>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["DemoApiUrl"] ?? string.Empty);
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddPolicyHandler(HttpPolicies.GetRetryPolicy).AddPolicyHandler(HttpPolicies.GetCircuitBreakerPolicy());

builder.Services.AddSwagger();

builder.Services.AddRedis(builder.Configuration);

builder.Services.AddMasTransit(builder.Configuration);

builder.Services.AddBackgroundService();

var app = builder.Build();

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