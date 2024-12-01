using Application.DTOs.Pedido;
using Application.UseCase.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoUseCase _pedidoUseCase;
        public PedidoController(IPedidoUseCase pedidoUseCase)
        {
            _pedidoUseCase = pedidoUseCase;
        }        

        [HttpPost]
        [Route("checkout")]
        public async Task<IActionResult> Inserir(CadastrarPedidoDto pedidoDto)
        {
            try
            {
                return Ok(await _pedidoUseCase.Inserir(pedidoDto));
            }
            catch(Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpGet]
        [Route("consultar-status-pagamento/{pedidoId}")]
        public async Task<IActionResult> AtualizarStatus(long pedidoId)
        {
            try
            {
                return Ok(await _pedidoUseCase.ConsultarStatusPagamento(pedidoId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }        

        [HttpPut]
        [Route("atualizar-status/{id}/{status}")]
        public async Task<IActionResult> AtualizarStatus(long id, int status)
        {
            try
            {
                return Ok(await _pedidoUseCase.AtualizarStatus(id, status));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {          
            return Ok(await _pedidoUseCase.Listar());
        }
    }
}
