using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExamenEximicion.Libreria.Models;
using ExamenEximicion.RegistroInaiso.Models;

namespace RegistroInaiso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly DataContext _context;

        public ReservationsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
          if (_context.Reservations == null)
          {
              return NotFound();
          }
            return await _context.Reservations.ToListAsync();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(DateTime id)
        {
          if (_context.Reservations == null)
          {
              return NotFound();
          }
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // PUT: api/Reservations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(DateTime id, Reservation reservation)
        {
            if (id != reservation.ReservedAt)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
          if (_context.Reservations == null)
          {
              return Problem("Entity set 'DataContext.Reservations'  is null.");
          }
            _context.Reservations.Add(reservation);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ReservationExists(reservation.ReservedAt))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetReservation", new { id = reservation.ReservedAt }, reservation);
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(DateTime id)
        {
            if (_context.Reservations == null)
            {
                return NotFound();
            }
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetReservationsByUser")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByUser(long user_Id, int year, int month)
        {
            if (_context.Reservations == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservations
                .Where(r => r.User_Id == user_Id &&
                            r.ReservedAt.Year == year &&
                            r.ReservedAt.Month == month)
                .ToListAsync();

            if (!reservations.Any())
            {
                return NotFound(); // Puedes cambiar esto a un mensaje más adecuado si lo deseas
            }

            return reservations;
        }

        [HttpPost("CreateReservation")]
        public async Task<ActionResult<Reservation>> CreateReservation(DateTime reservedAt, long userId, long appId)
        {
            if (_context.Reservations == null)
            {
                return Problem("Entity set 'DataContext.Reservations' is null.");
            }

            // Verificar si ya existe una reserva para la fecha, usuario y aplicación proporcionados
            var existingReservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.User_Id == userId && r.App_Id == appId && r.ReservedAt.Date == reservedAt.Date);

            if (existingReservation != null)
            {
                return Conflict("Ya existe una reserva para esta fecha, usuario y aplicación.");
            }

            // Crear una nueva reserva
            var reservation = new Reservation
            {
                ReservedAt = reservedAt,
                User_Id = userId,
                App_Id = appId,
                // Puedes configurar otras propiedades de la reserva aquí si es necesario
            };

            _context.Reservations.Add(reservation);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("Error al crear la reserva.");
            }

            return CreatedAtAction("GetReservation", new { id = reservation.ReservedAt }, reservation);
        }


        private bool AppExists(long id)
        {
            return (_context.Apps?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool ReservationExists(DateTime id)
        {
            return (_context.Reservations?.Any(e => e.ReservedAt == id)).GetValueOrDefault();
        }
    }
}
