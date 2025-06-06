namespace Bico.Models
{
    public class EfipayCredentials
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public bool sandbox { get; set; }
        public string certificate { get; set; }
        public string pix_key { get; set; }
    }
}
