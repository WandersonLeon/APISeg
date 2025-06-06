using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace APICrud.Modelos
{
    public class Tarefa
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Concluida { get; set; }

        /*
        Chave estrangeira para o usuário que criou a tarefa. 
        Permite que cada tarefa esteja associada a um usuário específico 
        e facilita a consulta de tarefas por usuário.
        */
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}