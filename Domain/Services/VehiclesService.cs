using Azure;
using Microsoft.EntityFrameworkCore;
using minimal_api.Databases;
using minimal_api.Entity;
using minimal_api.Interfaces;

namespace minimal_api.Services;

public class VehicleService : IVehicleService
{
    private readonly DatabaseContext _context;

    public VehicleService(DatabaseContext context)
    {
        _context = context;
    }

    public List<Vehicle> All(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
        }

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        return query.ToList();
    }

    public void IncludeVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? FindVehicleById(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}