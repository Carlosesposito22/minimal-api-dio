using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infra.Db;

namespace minimal_api.Domain.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public Admin Login(LoginDTO loginDTO)
        {
            return _context.Administradores
                .Where(a => (a.Email == loginDTO.Email && a.Senha == loginDTO.Senha))
                .FirstOrDefault();
        }
    }
}
