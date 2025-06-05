using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICrud.Migrations
{
    /// <inheritdoc />
    public partial class ConcluidaToTarefa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Concluida",
                table: "Tarefas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concluida",
                table: "Tarefas");
        }
    }
}
