using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;

        public EventoController(IProAgilRepository repo)
        {
            this._repo = repo;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _repo.GetAllEventoAsync(true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }  

        // GET api/values
        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId)
        {
            try
            {
                var results = await _repo.GetEventoAsyncById(eventoId, true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }   

        // GET api/values
        [HttpGet("getByTema/{tema}")]
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var results = await _repo.GetAllEventoAsyncByTema(tema, true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }  

        [HttpPost]
        public async Task<IActionResult> Post(Evento model)
        {
            try
            {
                this._repo.Add<Evento>(model);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", model);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        } 

        [HttpPut]
        public async Task<IActionResult> Put(int eventoId, Evento model)
        {
            try
            {
                var evento = await this._repo.GetEventoAsyncById(eventoId, false);
                if (evento == null)
                    return NotFound();
                this._repo.Update<Evento>(model);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", model);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        }                                              


        [HttpDelete]
        public async Task<IActionResult> Delete(int eventoId)
        {
            try
            {
                var evento = await this._repo.GetEventoAsyncById(eventoId, false);
                if (evento == null)
                    return NotFound();
                this._repo.Delete<Evento>(evento);
                if (await _repo.SaveChangesAsync())
                    return Ok();
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        }           
    }    
}