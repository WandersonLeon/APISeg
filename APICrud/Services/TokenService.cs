using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APICrud.Modelos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace APICrud.Services
{
    public static class TokenService
    {
        public static string GerarToken(Usuario usuario)
        {
            //Chave de assinatura do Token, como não vou deixar a API online, vou deixar a chave aqui de exemplo.
            var key = Encoding.ASCII.GetBytes("TresPratosDeTrigoParaTresTigresTristes");
            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                       {
                new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                           new SymmetricSecurityKey(key),
                           SecurityAlgorithms.HmacSha256Signature)
            };
            

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            return tokenHandler.WriteToken(token);
        }
    }
}