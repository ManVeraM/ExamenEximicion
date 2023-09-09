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
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'DataContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

       // GET: api/Users/Reservations/{id}
[HttpGet("Reservations/{id}")]
public async Task<ActionResult<UserReservations>> GetUserReservations(long id)
{
    if (_context.Users == null)
    {
        return NotFound();
    }

    var user = await _context.Users.FindAsync(id);

    if (user == null)
    {
        return NotFound();
    }

    // Obtener la fecha actual y las fechas límite para el último mes y el último año
    var currentDate = DateTime.Now;
    var lastMonthDate = currentDate.AddMonths(-1);
    var lastYearDate = currentDate.AddYears(-1);

    // Contar las reservaciones del último mes y el último año para el usuario
    var reservationsLastMonth = await _context.Reservations
        .Where(r => r.User_Id == id && r.ReservedAt >= lastMonthDate && r.ReservedAt <= currentDate)
        .CountAsync();

    var reservationsLastYear = await _context.Reservations
        .Where(r => r.User_Id == id && r.ReservedAt >= lastYearDate && r.ReservedAt <= currentDate)
        .CountAsync();

    // Crear un objeto que contenga la información requerida
    var userReservations = new UserReservations
    {
        UserName = user.Name,
        ReservationsLastMonth = reservationsLastMonth,
        ReservationsLastYear = reservationsLastYear
    };

    return userReservations;

}
    [HttpPost("ImportUsers")]
    public async Task<IActionResult> ImportUsers([FromBody] List<User> users)
    {
        if (users == null || users.Count == 0)
        {
            return BadRequest("La lista de usuarios está vacía o es nula.");
        }

        try
        {
            foreach (var user in users)
            {
                _context.Users.Add(new User
                {
                    Name = user.Name,
                    // Configura otras propiedades del usuario según sea necesario
                });
            }

            await _context.SaveChangesAsync();
            return Ok("Usuarios importados exitosamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
