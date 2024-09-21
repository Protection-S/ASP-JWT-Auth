using Microsoft.AspNetCore.Mvc;
using WebApiMicroServices.Data;
using WebApiMicroServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
namespace DanyaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocksController : ControllerBase
    {
        private readonly Context _context;

        public SocksController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sock>>> GetSocks()
        {
            return await _context.Socks.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sock>> GetSock(int id)
        {
            var sock = await _context.Socks.FindAsync(id);
            if (sock == null)
            {
                return NotFound();
            }
            return sock;
        }

        [HttpPost]
        public async Task<ActionResult<Sock>> CreateSock(Sock sock)
        {
            _context.Socks.Add(sock);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSock), new { id = sock.Id }, sock);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSock(int id, Sock sock)
        {
            if (id != sock.Id)
            {
                return BadRequest();
            }

            _context.Entry(sock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SockExists(id))
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchSock(int id, [FromBody] JsonPatchDocument<Sock> patchDoc)
        {
            var sock = await _context.Socks.FindAsync(id);
            if (sock == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(sock);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSock(int id)
        {
            var sock = await _context.Socks.FindAsync(id);
            if (sock == null)
            {
                return NotFound();
            }

            _context.Socks.Remove(sock);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SockExists(int id)
        {
            return _context.Socks.Any(e => e.Id == id);
        }
    }
}
