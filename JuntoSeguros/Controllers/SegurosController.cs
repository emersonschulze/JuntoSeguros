using Junto.Dominio;
using JuntoSeguros.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuntoSeguros.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize("Bearer")]
    public class SegurosController : ControllerBase
    {
        private SegurosService _service;

        public SegurosController(SegurosService service)
        {
            _service = service;
        }


        /// <summary>
        /// Lista todos os seguro e os valores.
        /// </summary>
        /// <returns>Valor Unidades de medidas</returns>
        /// <response code="200">Retorna os seguros e os valores correspondentes cadastradas</response>
        [HttpGet]
        public IEnumerable<Seguros> Get()
        {
            return _service.ListarTodos();
        }

        // GET api/v1/seguro/{id}
        /// <summary>
        /// Lista um seguro com seu valor respectivo filtrado por id.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     GET api/v1/Seguros/{id}
        ///     {
        ///        "Id": int,
        ///        "Nome": "string",
        ///        "Descricao": "string",
        ///        "Preco": double
        ///     }
        ///
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>seguro localizada</returns>
        /// <response code="200">Retorna o registro encontrado pelo id</response>

        [HttpGet("{id}")]
        public ActionResult<Seguros> Get(int id)
        {
            var seguro = _service.Obter(id);
            if (seguro != null)
                return seguro;
            else
                return NotFound();
        }

        // POST api/v1/seguro
        /// <summary>
        /// Cria um seguro com seu valor respectivo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     POST api/v1/seguro
        ///     {
        ///         "Nome": "string",
        ///        "Descricao": "string",
        ///        "Preco": double
        ///     }
        ///
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Novo seguro criado</returns>
        /// <response code="201">Retorna o novo registro criado</response>
        /// <response code="400">Se o registro não for criado</response>
        [HttpPost]
        public Resultado Post([FromBody] Seguros seguro)
        {
            return _service.Incluir(seguro);
        }

        // PUT api/v1/seguro/{id}
        /// <summary>
        /// Atualiza um seguro com seu valor respectivo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     PUT api/v1/seguro/{id}
        ///     {
        ///        "Nome": "string",
        ///        "Descricao": "string",
        ///        "Preco": double
        ///     }
        ///
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Atualiza seguro</returns>
        /// <response code="200">Retorna o registro atualizado</response>
        /// <response code="400">Se o registro não for atualizado</response>

        [HttpPut("{id}")]
        public Resultado Put([FromBody] Seguros seguro)
        {
            return _service.Atualizar(seguro);

        }


        // DELETE api/v1/seguro/{id}
        /// <summary>
        /// Deleta um seguro com seu valor respectivo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     DELETE api/v1/seguro/{id}
        ///     {
        ///        "Nome": "string",
        ///        "Descricao": "string",
        ///        "Preco": "double"
        ///     }
        ///
        /// </remarks>
        /// <param name="value"></param>
        /// <returns>Deleta seguro</returns>
        /// <response code="201">Retorna sucess </response>
        /// <response code="400">Erro Se o registro não for deletado</response>
        [HttpDelete("{id}")]
        public Resultado Delete(int id)
        {
            return _service.Excluir(id);
        }
    }
}