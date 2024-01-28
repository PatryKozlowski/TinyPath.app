using Serilog;
using TinyPath.Application.Logic.Abstractions;
using TinyPath.Infrastructure;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssemblyContaining(typeof(BaseCommandHandler)));

var app = builder.Build();

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

app.MapControllers();

app.Run();