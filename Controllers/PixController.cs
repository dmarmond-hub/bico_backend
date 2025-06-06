using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PixController : ControllerBase
{
    private readonly EfipayService _efipayService;

    public PixController(EfipayService efipayService)
    {
        _efipayService = efipayService;
    }

    [HttpPost("criar")]
    public IActionResult CriarPix([FromBody] PixRequest request)
    {
        try
        {
            var response = _efipayService.CriarCobrancaPix(request.Cpf, request.Nome, request.Valor, request.Descricao);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET: api/pix/cobrancas?inicio=2025-04-01T00:00:00Z&fim=2025-04-10T00:00:00Z
    [HttpGet("cobrancas")]
    public IActionResult ConsultarCobrancas([FromQuery] string inicio, [FromQuery] string fim)
    {
        try
        {
            var resultado = _efipayService.ConsultarCobrancasPix(inicio, fim);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = ex.Message });
        }
    }
}

public class PixRequest
{
    public string Cpf { get; set; }
    public string Nome { get; set; }
    public string Valor { get; set; }
    public string Descricao { get; set; }
}
