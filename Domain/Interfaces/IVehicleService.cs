using minimal_api.Entity;
namespace minimal_api.Interfaces;

public interface IVehicleService
{
    List<Vehicle> All(int? pagina = 1, string? nome = null, string? marca = null);
    
    Vehicle? FindVehicleById(int id);

    void IncludeVehicle(Vehicle vehicle);

    void UpdateVehicle(Vehicle vehicle);
    
    void RemoveVehicle(Vehicle vehicle);
}