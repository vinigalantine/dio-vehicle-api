using DioVehicleApi.Domain.Interfaces;

namespace DioVehicleApi.Domain.Entities;

public class Vehicle : BaseEntity<Guid>, ISoftDeletable
{
    
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public Guid ModelId { get; set; }
    
    public Model Model { get; set; } = null!;
}
