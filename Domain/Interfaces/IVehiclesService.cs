using System.ComponentModel;
using minimal_api.DTOs;
using minimal_api.Entity;

namespace minimal_api.Interfaces;

public interface IVehiclesService
{
    List<Vehicle> All(int pagina = 1, string? nome = null, string? marca = null);
    
    Vehicle? SearchVehicleById(int id);

    void IncludeVehicle(Vehicle vehicle);

    void UpdateVehicle(Vehicle vehicle);
    
    void RemoveVehicle(Vehicle vehicle);
}