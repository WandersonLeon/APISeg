using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrud.Modelos;
using Microsoft.EntityFrameworkCore;

namespace APICrud.Data
{
    public class AppDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração do DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // DbSets representa as tabelas do banco de dados
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; } 
    }
}