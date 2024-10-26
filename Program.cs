using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

app.MapPost("/admins/login", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>{
    var validacao = new ValidationErrors(){
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email))
    {
        validacao.Messages.Add("O e-mail não pode ser vazio!");
    }

    if (string.IsNullOrEmpty(adminDTO.Senha))
    {
        validacao.Messages.Add("A senha não pode ser vazia!");
    }

    if (string.IsNullOrEmpty(adminDTO.Perfil))
    {
        validacao.Messages.Add("O perfil não pode ser vazio!");
    }

    if (validacao.Messages.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var admin = new Admin{
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString(),
    };

    adminService.Include

}).WithTags("Admins");
#endregion

#region Vehicles
ValidationErrors validateDTO(VehicleDTO vehicleDTO)
{
    var validacao = new ValidationErrors(){
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Nome))
    {
        validacao.Messages.Add("O nome não pode ser vazio!");
    }

    if (string.IsNullOrEmpty(vehicleDTO.Marca))
    {
        validacao.Messages.Add("A marca não pode ser vazia!");
    }

    if (vehicleDTO.Ano < 2000)
    {
        validacao.Messages.Add("Veículo muito antigo, aceito somente anos superiores a 2000.");
    }

    return validacao;
}

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>{

    var validacao = validateDTO(vehicleDTO);

    if (validacao.Messages.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

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

    var validacao = validateDTO(vehicleDTO);
    
    if (validacao.Messages.Count > 0)
    {
        return Results.BadRequest(validacao);
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