using APICrud.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add os serviços do app
builder.Services.AddControllers();

// Conexao com o SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticação com JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                //Chave de assinatura do Token, como não vou deixar a API online, vou deixar a chave aqui de exemplo.
                Encoding.ASCII.GetBytes("TresPratosDeTrigoParaTresTigresTristes")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Swagger (Ainda não entendi direito)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de autenticação/autorização
app.UseAuthentication();
app.UseAuthorization();

//Rotas dos controladores
app.MapControllers();

app.Run();
