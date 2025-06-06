namespace Bico.Models
{
    public class BicoDatabaseSettings // A classe BicoDatabaseSettings é usada para armazenar os valores de propriedade appsettings.json do arquivo BicoDatabase 
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsuariosCollectionName { get; set; } = null!;
        public string ServicosCollectionName { get; set; } = null!;
        public string ContratosCollectionName { get; set; } = null!;
        public string PagamentosCollectionName { get; set; } = null!;
        public string ReviewsCollectionName { get; set; } = null!;
        public string LikesCollectionName { get; set; } = null!;



    }
}
