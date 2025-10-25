using System;

namespace DioVehicleApi.Domain.Entities;

public abstract class BaseEntity<T>
{
    public required T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
