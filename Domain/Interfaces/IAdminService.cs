using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdminService
    {
        Admin Login(LoginDTO loginDTO);

        Admin Create(Admin admin);

        List<Admin> GetAll(int page = 1);

        Admin FindById(int id);

        string GerarTokenJwt(Admin admin, string key);
    }
}
