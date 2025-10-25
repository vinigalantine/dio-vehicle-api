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
    public string DeletedBy { get; set; } = string.Empty;

    public Model() 
    { 
        Id = Guid.NewGuid();
    }

    public Model(string name, Guid brandId)
    {
        Id = Guid.NewGuid();
        Name = name;
        BrandId = brandId;
    }

    public Model(string name, Brand brand)
    {
        Id = Guid.NewGuid();
        Name = name;
        Brand = brand;
        BrandId = brand.Id;
    }
}