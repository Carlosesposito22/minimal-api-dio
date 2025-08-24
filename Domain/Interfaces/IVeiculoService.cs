using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IVeiculoService
    {
        List<Veiculo> ListAll(int page = 1, string nome = null, string marca = null);

        Veiculo FindById(int id);

        void Create(Veiculo veiculo);

        void Update(Veiculo veiculo);

        void Delete(Veiculo veiculo);
    }
}
