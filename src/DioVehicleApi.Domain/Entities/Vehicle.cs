using DioVehicleApi.Domain.Interfaces;

namespace DioVehicleApi.Domain.Entities;

public class Vehicle : BaseEntity<Guid>, ISoftDeletable
{
    public Guid ModelId { get; set; }
    
    public Model Model { get; set; } = null!;
    
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;

    public Vehicle() 
    { 
        Id = Guid.NewGuid();
    }

    public Vehicle(Guid modelId, int year, string color, string licensePlate)
    {
        Id = Guid.NewGuid();
        ModelId = modelId;
        Year = year;
        Color = color;
        LicensePlate = licensePlate;
    }

    public Vehicle(Model model, int year, string color, string licensePlate)
    {
        Id = Guid.NewGuid();
        Model = model;
        ModelId = model.Id;
        Year = year;
        Color = color;
        LicensePlate = licensePlate;
    }
}
