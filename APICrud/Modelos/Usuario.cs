using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICrud.Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }

        public ICollection<Tarefa> Tarefas { get; set; }
    }
}