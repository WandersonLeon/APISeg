using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrud.Data;
using APICrud.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

            var usuariosViewModel = usuarios.Select(u => new UsuarioViewModel
            {
                Id = u.Id,
                Email = u.Email
            });

            return Ok(usuariosViewModel);
        }

        [HttpGet("usuarios/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (usuario == null)
                return NotFound($"Usuário com ID {id} não encontrado.");

            var usuarioViewModel = new UsuarioViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email
            };

            return Ok(usuarioViewModel);
        }

        [HttpPost("usuarios")]
        public async Task<IActionResult> PostAsync([FromBody] CreateUsuario model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            try
            {
                // Verifica se já existe um usuário com esse e-mail
                var emailExistente = await _context.Usuarios
                    .AsNoTracking()
                    .AnyAsync(x => x.Email == model.Email);

                if (emailExistente)
                    return BadRequest("Já existe um usuário com este e-mail.");

                var usuario = new Usuario
                {
                    Email = model.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha)
                };

                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();

                return Created($"v1/usuarios/{usuario.Id}", new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao salvar usuário: {e.Message}");
            }
        }


        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] CreateUsuario model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados do usuário não podem ser nulos.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            if (usuario == null)
                return NotFound($"Usuário com ID {id} não encontrado.");

            usuario.Email = model.Email ?? usuario.Email;
            if (!string.IsNullOrEmpty(model.Senha))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha);
            }

            try
            {
                // Verifica se o e-mail já existe para outro usuário 
                if (!string.IsNullOrEmpty(usuario.Email = model.Email))
                {
                    var emailExistente = await _context.Usuarios
                        .AsNoTracking()
                        .AnyAsync(x => x.Email == usuario.Email && x.Id != id);
                    if (emailExistente)
                    {
                        return BadRequest("Já existe um usuário com este e-mail.");
                    }
                }
                // Verifica se o e-mail é nulo ou vazio
                else if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(usuario.Email))
                {
                    return BadRequest("E-mail não pode ser nulo.");
                }
                // Altera o e-mail se fornecido
                else
                {
                    usuario.Email = model.Email;
                }

                // Atualiza a senha se fornecida
                if (!string.IsNullOrEmpty(model.Senha))
                {
                    usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha);
                }
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok(usuario);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {e.Message}");
            }
        }

    }
}
