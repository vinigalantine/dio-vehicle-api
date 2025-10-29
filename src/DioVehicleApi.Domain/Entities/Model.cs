using DioVehicleApi.Domain.Interfaces;

namespace DioVehicleApi.Domain.Entities;

public class Model : BaseEntity<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    
    public Guid BrandId { get; set; }
    
    public Brand Brand { get; set; } = null!;
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}