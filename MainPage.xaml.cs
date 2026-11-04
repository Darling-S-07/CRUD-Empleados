using System.Globalization;
using System.Text.RegularExpressions;
using CRUD_Empleados.Data;
using CRUD_Empleados.Models;

namespace CRUD_Empleados
{
    public partial class MainPage : ContentPage
    {
        private static readonly Regex CorreoRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private readonly SQLiteDatabase _db;

        // Empleado seleccionado en la lista (null = se está registrando uno nuevo).
        private Empleados? _seleccionado;

        public MainPage(SQLiteDatabase db)
        {
            InitializeComponent();
            _db = db;
            FechaIngresoPicker.Date = DateTime.Today;
        }

        // Al abrir la página se crea la BD/tabla (primer acceso) y se carga la lista.
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarEmpleadosAsync();
        }

        private async Task CargarEmpleadosAsync()
        {
            var empleados = await _db.ObtenerEmpleadosAsync();
            EmpleadosListView.ItemsSource = empleados;
            SinEmpleadosLabel.IsVisible = empleados.Count == 0;
        }

        // ---------- Guardar (insertar o actualizar) ----------
        private async void OnGuardarClicked(object? sender, EventArgs e)
        {
            var nombres = NombresEntry.Text?.Trim() ?? "";
            var apellidos = ApellidosEntry.Text?.Trim() ?? "";
            var puesto = PuestoEntry.Text?.Trim() ?? "";
            var correo = CorreoEntry.Text?.Trim() ?? "";

            // Validaciones
            if (string.IsNullOrWhiteSpace(nombres))
            {
                await Avisar("El campo Nombres es obligatorio.");
                return;
            }
            if (string.IsNullOrWhiteSpace(apellidos))
            {
                await Avisar("El campo Apellidos es obligatorio.");
                return;
            }
            if (string.IsNullOrWhiteSpace(puesto))
            {
                await Avisar("El campo Puesto es obligatorio.");
                return;
            }
            if (!decimal.TryParse(SalarioEntry.Text, NumberStyles.Number,
                                  CultureInfo.CurrentCulture, out var salario))
            {
                await Avisar("El salario debe ser un número válido.");
                return;
            }
            if (salario <= 0)
            {
                await Avisar("El salario debe ser mayor que cero.");
                return;
            }
            if (!CorreoRegex.IsMatch(correo))
            {
                await Avisar("Ingrese un correo con formato válido (ej. nombre@dominio.com).");
                return;
            }

            // Compatible tanto si DatePicker.Date es DateTime como DateTime? (.NET 10).
            object? fechaObj = FechaIngresoPicker.Date;
            var fecha = fechaObj is DateTime f ? f : DateTime.Today;

            if (_seleccionado is null)
            {
                var nuevo = new Empleados
                {
                    Nombres = nombres,
                    Apellidos = apellidos,
                    Puesto = puesto,
                    Salario = salario,
                    Correo = correo,
                    FechaIngreso = fecha
                };
                await _db.GuardarEmpleadoAsync(nuevo);
                await Avisar("Empleado registrado correctamente.", "Listo");
            }
            else
            {
                _seleccionado.Nombres = nombres;
                _seleccionado.Apellidos = apellidos;
                _seleccionado.Puesto = puesto;
                _seleccionado.Salario = salario;
                _seleccionado.Correo = correo;
                _seleccionado.FechaIngreso = fecha;
                await _db.ActualizarEmpleadoAsync(_seleccionado);
                await Avisar("Empleado actualizado correctamente.", "Listo");
            }

            LimpiarFormulario();
            await CargarEmpleadosAsync();
        }

        // ---------- Seleccionar para editar ----------
        private void OnEmpleadoSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is not Empleados emp)
                return;

            _seleccionado = emp;
            NombresEntry.Text = emp.Nombres;
            ApellidosEntry.Text = emp.Apellidos;
            PuestoEntry.Text = emp.Puesto;
            SalarioEntry.Text = emp.Salario.ToString("0.##", CultureInfo.CurrentCulture);
            CorreoEntry.Text = emp.Correo;
            FechaIngresoPicker.Date = emp.FechaIngreso;

            GuardarButton.Text = "Actualizar";
            EliminarButton.IsEnabled = true;
        }

        // ---------- Eliminar con confirmación ----------
        private async void OnEliminarClicked(object? sender, EventArgs e)
        {
            if (_seleccionado is null)
                return;

            bool confirmar = await DisplayAlertAsync(
                "Confirmar eliminación",
                $"¿Desea eliminar a {_seleccionado.NombreCompleto}?",
                "Sí, eliminar", "Cancelar");

            if (!confirmar)
                return;

            await _db.EliminarEmpleadoAsync(_seleccionado);
            LimpiarFormulario();
            await CargarEmpleadosAsync();
        }

        private void OnLimpiarClicked(object? sender, EventArgs e) => LimpiarFormulario();

        // ---------- Utilidades ----------
        private void LimpiarFormulario()
        {
            _seleccionado = null;
            NombresEntry.Text = string.Empty;
            ApellidosEntry.Text = string.Empty;
            PuestoEntry.Text = string.Empty;
            SalarioEntry.Text = string.Empty;
            CorreoEntry.Text = string.Empty;
            FechaIngresoPicker.Date = DateTime.Today;

            EmpleadosListView.SelectedItem = null;   // permite volver a seleccionar el mismo
            GuardarButton.Text = "Guardar";
            EliminarButton.IsEnabled = false;
        }

        private Task Avisar(string mensaje, string titulo = "Validación")
            => DisplayAlertAsync(titulo, mensaje, "OK");
    }
}