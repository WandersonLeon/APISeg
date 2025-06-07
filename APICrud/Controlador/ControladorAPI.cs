using APICrud.Data;
using APICrud.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using APICrud.Services;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet("usuarios/me")]
        public async Task<IActionResult> GetMeuUsuario()
        {
            var userId = int.Parse(User.Identity.Name);

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

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
                if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
                    return BadRequest("Já existe um usuário com esse e-mail.");

                var usuario = new Usuario
                {
                    Email = model.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha),
                    Role = "user"
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

        [Authorize]
        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] UpdateUsuario model)
        {
            var userId = int.Parse(User.Identity.Name);
            var userRole = User.FindFirst("role")?.Value;

            if (userRole != "admin" && userId != id)
                return Unauthorized("Você não tem permissão para alterar este usuário.");

            // Verifica se o ID é valido
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            // Verifica se o usuário existe
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound($"Usuário com ID {id} não encontrado.");

            // Verifica senha atual
            if (!BCrypt.Net.BCrypt.Verify(model.SenhaAtual, usuario.SenhaHash))
                return BadRequest("Senha atual incorreta.");

            // Verifica se email novo ja esta em uso
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.EmailNovo && u.Id != id))
                return BadRequest("Email já está em uso por outro usuário.");

            // Atualiza email
            usuario.Email = model.EmailNovo;

            // Se o usuario digitar uma nova senha, atualiza o hash
            if (!string.IsNullOrEmpty(model.NovaSenha))
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha);

            try
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                return Ok(new { usuario.Id, usuario.Email });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {e.Message}");
            }
        }

        [Authorize]
        [HttpDelete("usuarios/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id)
        {

            var userId = int.Parse(User.Identity.Name);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            //Verifica se o usuário é admin ou se é o próprio usuário que está tentando deletar
            if (userRole != "admin" && userId != id)
                return Unauthorized("Você não tem permissão para alterar este usuário.");

            if (!ModelState.IsValid)
                return BadRequest("Dados Invalidos.");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return NotFound($"Usuario com ID {id} não encontrado.");

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return Ok($"Usuario com ID {id} foi removido com sucesso.");
            }

            catch (Exception e)
            {
                return StatusCode(500, $"Erro ao atualizar usuário: {e.Message}");
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] ModeloLogin model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            // Verifica se o usuário existe
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (usuario == null)
                return Unauthorized("Usuário ou senha incorretos.");

            // Se o usuario tiver bloqueio e o tempo de bloqueio ainda não tiver passado, retorna erro
            if (usuario.Bloqueio.HasValue && usuario.Bloqueio > DateTime.UtcNow)
                return Unauthorized($"Conta bloqueada até {usuario.Bloqueio.Value}.");


            // Verifica se a senha informada corresponde ao hash armazenado
            bool senhaValida = BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash);

            //Cada tentativa de senha incorreta adiciona 1 tentativa, 3 tentativas incorretas bloqueiam.
            if (!senhaValida)
            {
                usuario.TentativasLogin++;
                if (usuario.TentativasLogin > 3)
                {
                    usuario.Bloqueio = DateTime.UtcNow.AddMinutes(15); //Bloqueia por 15min
                    usuario.TentativasLogin = 0;
                }
                return Unauthorized("Usuário ou senha incorretos.");
            }

            //Se usuario e senha forem validos, gera um Token
            usuario.TentativasLogin = 0; //Reseta tentativas de login
            usuario.Bloqueio = null; //Remove bloqueio se existir
            _context.Usuarios.Update(usuario);

            var token = TokenService.GerarToken(usuario);

            return Ok(new
            {
                token,
                usuario = new UsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email
                }
            });

        }
    }
}