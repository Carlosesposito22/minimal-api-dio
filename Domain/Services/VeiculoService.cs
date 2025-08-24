using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infra.Db;

namespace minimal_api.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly AppDbContext _context;
        public VeiculoService(AppDbContext context)
        {
            _context = context;
        }

        public Veiculo FindById(int id)
        {
            return _context.Veiculos.FirstOrDefault(v => v.Id == id);
        }

        public List<Veiculo> ListAll(int page = 1, string nome = null, string marca = null)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.ToLower().Contains(nome));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.ToLower().Contains(marca));
            }

            int itensPorPage = 10;

            return query.Skip((page - 1) * itensPorPage).Take(itensPorPage).ToList();
        }

        public void Create(Veiculo veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
        }

        public void Delete(Veiculo veiculo)
        {
            _context.Remove(veiculo);
            _context.SaveChanges();
        }

        public void Update(Veiculo veiculo)
        {
            _context.Update(veiculo);
            _context.SaveChanges();
        }
    }
}
