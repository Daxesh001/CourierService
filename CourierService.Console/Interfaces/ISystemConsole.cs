namespace CourierService.Console.Interfaces
{
    public interface ISystemConsole
    {
        string? ReadLine();
        void Write(string value);
        void WriteLine(string? value = null);
    }
}