using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext context)
        {
            this._context = context;
            this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public void Add<T>(T entity) where T : class
        {
            this._context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this._context.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            this._context.Update(entity);
        }        

        public async Task<bool> SaveChangesAsync()
        {
            return (await this._context.SaveChangesAsync()) > 0;
        }

        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = this._context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);

            query = query.AsNoTracking()
                .OrderBy(c => c.Id);
            return await query.ToArrayAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes)
        {
            IQueryable<Evento> query = this._context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);

            query = query.AsNoTracking()
                         .OrderByDescending(c => c.DataEvento)
                         .Where(c => c.Tema.ToUpper().Contains(tema.ToUpper()));

            return await query.ToArrayAsync();       
        }

        public async Task<Evento> GetEventoAsyncById(int eventoId, bool includePalestrantes)
        {
            IQueryable<Evento> query = this._context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);

            query = query.AsNoTracking()
                        .Where(c => c.Id == eventoId);

            return await query.FirstOrDefaultAsync();       
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string name, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = this._context.Palestrantes
                .Include(c => c.RedesSociais);

            if (includeEventos)
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);

            query = query.AsNoTracking()
                         .Where(c => c.Nome.ToUpper().Contains(name.ToUpper()))
                         .OrderBy(c => c.Nome);

            return await query.ToArrayAsync();
        }

        public async Task<Palestrante> GetPalestranteAsync(int palestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = this._context.Palestrantes
                .Include(c => c.RedesSociais);

            if (includeEventos)
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);

            query = query.AsNoTracking()
                    .Where(c => c.Id == palestranteId);

            return await query.FirstOrDefaultAsync();           }
    }
}