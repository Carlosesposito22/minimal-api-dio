using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.DTOs
{
    public class AdminDTO
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public Role? Role { get; set; }
    }
}
