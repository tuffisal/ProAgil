using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PalestranteController : ControllerBase
    {
        private readonly IProAgilRepository _repo;

        public PalestranteController(IProAgilRepository repo)
        {
            this._repo = repo;
        }

        // GET api/values
        [HttpGet("{palestranteId}")]
        public async Task<IActionResult> Get(int palestranteId)
        {
            try
            {
                var results = await _repo.GetPalestranteAsync(palestranteId, true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }   

        // GET api/values
        [HttpGet("getByName/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var results = await _repo.GetAllPalestrantesAsyncByName(name, true);
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
        }  

        [HttpPost]
        public async Task<IActionResult> Post(Palestrante model)
        {
            try
            {
                this._repo.Add<Palestrante>(model);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/palestrante/{model.Id}", model);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        } 

        [HttpPut]
        public async Task<IActionResult> Put(int palestranteId, Palestrante model)
        {
            try
            {
                var palestrante = await this._repo.GetPalestranteAsync(palestranteId, false);
                if (palestrante == null)
                    return NotFound();
                this._repo.Update<Palestrante>(model);
                if (await _repo.SaveChangesAsync())
                    return Created($"/api/palestrante/{model.Id}", model);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        }                                              


        [HttpDelete]
        public async Task<IActionResult> Delete(int palestranteId)
        {
            try
            {
                var palestrante = await this._repo.GetPalestranteAsync(palestranteId, false);
                if (palestrante == null)
                    return NotFound();
                this._repo.Delete<Palestrante>(palestrante);
                if (await _repo.SaveChangesAsync())
                    return Ok();
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou!");
            }
            return BadRequest();
        }        }
}