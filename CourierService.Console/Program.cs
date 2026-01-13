using Microsoft.Extensions.DependencyInjection;
using CourierService.Application.Interfaces;
using CourierService.Application.Services;
using CourierService.Console.Interfaces;
using CourierService.Console.Services;

var services = new ServiceCollection();

// Register application services (reuse implementations)
services.AddScoped<ICostCalculator, CostCalculationService>();
services.AddScoped<IOfferService, OfferService>();
services.AddScoped<IDeliveryScheduler, DeliverySchedulingService>();

// Console abstractions / app
services.AddSingleton<ISystemConsole, SystemConsole>();
services.AddScoped<IConsoleApp, ConsoleApp>();

var provider = services.BuildServiceProvider();

// Run the app (single responsibility: Program only boots and runs)
var app = provider.GetRequiredService<IConsoleApp>();
await app.RunAsync();
