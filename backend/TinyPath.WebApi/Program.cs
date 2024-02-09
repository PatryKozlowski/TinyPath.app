using Serilog;
using TinyPath.Application;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Infrastructure;
using TinyPath.Infrastructure.Persistence.Hangfire;
using TinyPath.WebApi.Application.Auth;
using TinyPath.WebApi.Application.Link;
using TinyPath.WebApi.Middlewares;

const string APP_NAME = "TinyPath";

Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Application", APP_NAME)
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .Enrich.WithProperty("Application", APP_NAME)
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(x =>
    {
        var name = x.FullName;
        if (name is not null)
        {
            name = name.Replace("+", "-");
        }

        return name;
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddAuthDataProvider(builder.Configuration);
builder.Services.AddLinkStatsProvider();

builder.Services.AddControllers();

builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssemblyContaining(typeof(BaseCommandHandler)));

var app = builder.Build();

app.UseCustomMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    Log.Logger = new LoggerConfiguration()
        .Enrich.WithProperty("Application", $"DEV-{APP_NAME}")
        .Enrich.WithProperty("MachineName", Environment.MachineName)
        .Enrich.FromLogContext()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();
}

app.UseAuthorization();

app.MapControllers();

app.UseHangfire(builder.Configuration);

app.Run();