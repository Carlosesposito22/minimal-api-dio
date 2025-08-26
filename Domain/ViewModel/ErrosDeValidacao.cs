namespace minimal_api.Domain.ViewModel
{
    public class ErrosDeValidacao
    {
        public List<string> Menssagens { get; set; }

        public ErrosDeValidacao()
        {
            Menssagens = new List<string>();
        }
    }
}
