using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Databases;
using minimal_api.DTOs;
using minimal_api.Entity;
using minimal_api.Interfaces;
using minimal_api.ModelViews;
using minimal_api.Services;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home 
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
# endregion

#region Admins
app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>{
    if (adminService.Login(loginDTO) != null)
    {
        return Results.Ok("Login efetuado com sucesso!");
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithTags("Admins");
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>{
    
    var vehicle = new Vehicle{
        Nome = vehicleDTO.Nome,
        Marca = vehicleDTO.Marca,
        Ano = vehicleDTO.Ano,
    };

    vehicleService.IncludeVehicle(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? pagina, IVehicleService vehicleService) =>{
    
    var vehicles = vehicleService.All(pagina);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    
    var vehicle = vehicleService.FindVehicleById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>{
    
    var vehicle = vehicleService.FindVehicleById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    vehicle.Nome = vehicleDTO.Nome;
    vehicle.Marca = vehicleDTO.Marca;
    vehicle.Ano = vehicleDTO.Ano;
    
    vehicleService.UpdateVehicle(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>{
    
    var vehicle = vehicleService.FindVehicleById(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }
    
    vehicleService.RemoveVehicle(vehicle);

    return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion