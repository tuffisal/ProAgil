using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.API.Dtos;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;
        private readonly IMapper _mapper;

        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsync(true);
                var results = _mapper.Map<IEnumerable<EventoDto>>(eventos);
                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}!");
            }
        }  

        // GET api/values
        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(eventoId, true);
                var results = _mapper.Map<EventoDto>(evento);
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
                var eventos = await _repo.GetAllEventoAsyncByTema(tema, true);
                var results = _mapper.Map<IEnumerable<EventoDto>>(eventos);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }  

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);
                this._repo.Add<Evento>(evento);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        } 

        [HttpPut("{eventoId}")]
        public async Task<IActionResult> Put(int eventoId, EventoDto model)
        {
            try
            {
                var evento = await this._repo.GetEventoAsyncById(eventoId, false);
                if (evento == null)
                    return NotFound();

                _mapper.Map(model, evento);

                this._repo.Update<Evento>(evento);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        }                                              


        [HttpDelete("{eventoId}")]
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