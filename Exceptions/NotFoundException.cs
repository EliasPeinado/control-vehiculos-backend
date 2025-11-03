namespace ControlVehiculos.Exceptions;

public class NotFoundException : Exception
{
    public string ResourceName { get; }
    public object ResourceKey { get; }

    public NotFoundException(string resourceName, object resourceKey)
        : base($"{resourceName} con identificador '{resourceKey}' no fue encontrado.")
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }

    public NotFoundException(string message) : base(message)
    {
        ResourceName = string.Empty;
        ResourceKey = string.Empty;
    }
}
