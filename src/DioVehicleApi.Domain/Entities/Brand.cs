using DioVehicleApi.Domain.Interfaces;

namespace DioVehicleApi.Domain.Entities;

public class Brand : BaseEntity<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Model> Models { get; set; } = new List<Model>();
    
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}