namespace DeviceDiscovery.Interfaces
{
    public interface IHost
    {
        string Name { get; }
        int Port { get; }
    }
}