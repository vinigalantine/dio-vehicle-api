namespace DioVehicleApi.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with ID '{key}' was not found")
    {
    }
}
