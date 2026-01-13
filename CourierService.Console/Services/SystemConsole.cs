using CourierService.Console.Interfaces;

namespace CourierService.Console.Services
{
    public class SystemConsole : ISystemConsole
    {
        public string? ReadLine() => System.Console.ReadLine();
        public void Write(string value) => System.Console.Write(value);
        public void WriteLine(string? value = null) => System.Console.WriteLine(value);
    }
}