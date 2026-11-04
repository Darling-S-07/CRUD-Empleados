using SQLite;

namespace CRUD_Empleados.Models
{
    public class Empleados
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [NotNull]
        public string Puesto { get; set; } = string.Empty;

        [NotNull]
        public decimal Salario { get; set; }

        [NotNull, MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [NotNull]
        public DateTime FechaIngreso { get; set; }

        [Ignore]
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}