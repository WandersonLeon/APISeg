using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrud.Data;
using APICrud.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICrud.Controlador
{
    [ApiController]
    [Route(template: "v1")]
    public class ControladorAPI : ControllerBase
    {
        private readonly AppDbContext _context;

        public ControladorAPI(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usuarios")]
        public async Task<IActionResult> GetAsync()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();

            if (usuarios == null || !usuarios.Any())
                return NotFound("Nenhum usuário encontrado.");

            return Ok(usuarios);
        }

        [HttpGet("usuarios/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {

            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (usuario == null)
                return NotFound($"Usuário com ID {id} não encontrado.");

            return Ok(usuario);
        }

        [HttpPost("usuarios")]
        public async Task<IActionResult> PostAsync([FromBody] CreateUsuario usuario)
        {
            if (usuario == null)
                return BadRequest("Usuário não pode ser nulo.");

            try
            {
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();
                return Created($"v1/usuarios/{usuario.Id}", value: usuario);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao salvar usuário: {e.Message}");
            }

        }
    }
}