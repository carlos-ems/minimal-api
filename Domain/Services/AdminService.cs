using minimal_api.Databases;
using minimal_api.DTOs;
using minimal_api.Entity;
using minimal_api.Interfaces;

namespace minimal_api.Services;

public class AdminService : IAdminService
{
    private readonly DatabaseContext _context;

    public AdminService(DatabaseContext context)
    {
        _context = context;
    }

    public Admin? Login(LoginDTO loginDTO)
    {
        var adm = _context.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        
        return adm;
        ;
    }
}