using CourierService.Application.DTOs;
using CourierService.Application.Interfaces;
using CourierService.Console.Interfaces;
using CourierService.Domain.Entities;
using System.Globalization;

namespace CourierService.Console.Services
{
    public class ConsoleApp : IConsoleApp
    {
        private readonly IDeliveryScheduler _scheduler;
        private readonly ISystemConsole _console;

        public ConsoleApp(IDeliveryScheduler scheduler, ISystemConsole console)
        {
            _scheduler = scheduler;
            _console = console;
        }

        public async Task RunAsync()
        {
            _console.WriteLine("Courier Service - Console (interactive)");
            await RunInteractive();
        }

        private async Task RunInteractive()
        {
            var baseCost = ReadDecimal("Enter base delivery cost (e.g. 100): ");
            var count = ReadInt("How many packages? ");

            var packages = new List<Package>();
            for (int i = 1; i <= count; i++)
            {
                _console.WriteLine($"\nPackage #{i} — you may press Enter to use defaults where allowed.");
                var id = ReadString($"  Id (e.g. PKG{i}): ", defaultValue: $"PKG{i}");
                var weight = ReadDecimal($"  Weight (kg) (e.g. 50): ");
                var distance = ReadDecimal($"  Distance (km) (e.g. 30): ");
                var offer = ReadString("  Offer code (leave blank if none, e.g. OFR001): ", allowEmpty: true);

                packages.Add(new Package { Id = id, WeightKg = weight, DistanceKm = distance, OfferCode = string.IsNullOrWhiteSpace(offer) ? null : offer });
            }

            _console.WriteLine();
            var hasVehicle = ReadString("Do you want to provide vehicle info? (y/n): ", defaultValue: "n");
            Vehicle? vehicle = null;
            if (!string.IsNullOrEmpty(hasVehicle) && (hasVehicle.Equals("y", System.StringComparison.OrdinalIgnoreCase) || hasVehicle.Equals("yes", System.StringComparison.OrdinalIgnoreCase)))
            {
                int num = ReadInt("  Number of vehicles: ");
                decimal speed = ReadDecimal("  Max speed (km/h): ");
                decimal capacity = ReadDecimal("  Max carry weight (kg): ");
                vehicle = new Vehicle { NumberOfVehicles = num, MaxSpeedKmH = speed, MaxCarryWeightKg = capacity };
            }

            var result = _scheduler.ScheduleAndCalculate(baseCost, packages, vehicle ?? new Vehicle { NumberOfVehicles = 0 });
            PrintResult(result);

            await Task.CompletedTask;
        }

        private void PrintResult(DeliveryResultDto result)
        {
            _console.WriteLine("\n--- Delivery results ---");
            foreach (var p in result.Packages)
            {
                _console.WriteLine($"Package {p.Id}: DeliveryCost={p.DeliveryCost:0.00}, Discount={p.Discount:0.00}, Total={p.TotalCost:0.00}, ETA={p.ETAHours:0.00}h");
            }
            _console.WriteLine($"Total Cost: {result.TotalCost:0.00}");
            _console.WriteLine($"Max Delivery Time (hours): {result.MaxDeliveryTimeHours:0.00}");
        }

        private decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                _console.Write(prompt);
                var s = _console.ReadLine();
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v)) return v;
                _console.WriteLine("  Invalid number — try again.");
            }
        }

        private int ReadInt(string prompt)
        {
            while (true)
            {
                _console.Write(prompt);
                var s = _console.ReadLine();
                if (int.TryParse(s, out var v)) return v;
                _console.WriteLine("  Invalid integer — try again.");
            }
        }

        private string ReadString(string prompt, string? defaultValue = null, bool allowEmpty = false)
        {
            _console.Write(prompt);
            var s = _console.ReadLine();
            if (string.IsNullOrWhiteSpace(s))
            {
                if (allowEmpty) return string.Empty;
                if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;
                _console.WriteLine("  Value required — try again.");
                return ReadString(prompt, defaultValue, allowEmpty);
            }
            return s.Trim();
        }
    }
}