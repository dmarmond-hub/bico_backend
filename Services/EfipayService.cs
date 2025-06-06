using Efipay;
using System.Text.Json;
using Bico.Models; // Importa a classe que você criou
using Microsoft.Extensions.Configuration;

public class EfipayService
{
    private dynamic efi;
    private string pixKey;

    public EfipayService(IConfiguration configuration)
    {
        var credentialsPath = configuration["Efipay:CredentialsFile"];
        var json = File.ReadAllText(credentialsPath);

        // Agora usamos a classe tipada
        var credentials = JsonSerializer.Deserialize<EfipayCredentials>(json);

        efi = new EfiPay(
            credentials.client_id,
            credentials.client_secret,
            credentials.sandbox,
            credentials.certificate
        );

        pixKey = credentials.pix_key;
    }

    //PIX
    public dynamic CriarCobrancaPix(string cpf, string nome, string valor, string descricao)
    {
        var body = new
        {
            calendario = new { expiracao = 3600 },
            devedor = new { cpf = cpf, nome = nome }, //CPF deve ser um CPF válido
            valor = new { original = valor }, // valor deve ser do formato 20.00
            chave = pixKey,
            solicitacaoPagador = descricao
        };

        return efi.PixCreateImmediateCharge(null, body);
    }

    public dynamic ConsultarCobrancasPix(string inicio, string fim)
    {
        var parametros = new
        {
            inicio = inicio,
            fim = fim
        };

        return efi.PixListCharges(parametros);
    }









}
