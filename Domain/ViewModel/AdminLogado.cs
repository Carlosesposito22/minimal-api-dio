using minimal_api.Domain.Enuns;

namespace minimal_api.Domain.ViewModel
{
    public record AdminLogado
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
