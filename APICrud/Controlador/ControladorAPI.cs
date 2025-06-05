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
        public async Task<IActionResult> Get()
        {
            var usuarios = await _context.Usuarios.AsNoTracking().ToListAsync();

            if (usuarios == null || !usuarios.Any())
                return NotFound("Nenhum usuário encontrado.");

            return Ok(usuarios);
        }
    }
}