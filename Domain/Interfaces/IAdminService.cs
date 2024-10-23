using minimal_api.DTOs;
using minimal_api.Entity;

namespace minimal_api.Interfaces;

public interface IAdminService
{
    Admin? Login(LoginDTO loginDTO);
}