using Microsoft.IdentityModel.Tokens;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ViewModel;
using minimal_api.Infra.Db;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

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

        public Admin Create(Admin admin)
        {
            _context.Administradores.Add(admin);
            _context.SaveChanges();

            return admin;
        }

        public List<Admin> GetAll(int page = 1)
        {
            var query = _context.Administradores.AsQueryable();
            int itensPorPage = 10;

            return query.Skip((page - 1) * itensPorPage).Take(itensPorPage).ToList();
        }

        public Admin FindById(int id)
        {
            return _context.Administradores.FirstOrDefault(a => a.Id == id);
        }

        public string GerarTokenJwt(Admin admin, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim("Email", admin.Email),
                new Claim("Role", admin.Role),
                new Claim(ClaimTypes.Role, admin.Role)
            };

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
