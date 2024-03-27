using Microsoft.AspNetCore.Mvc;
using appweb_MBC.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace appweb_MBC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string nombre, string apellido, int edad, string genero, string Correo_Electronico, string departamento, string cargo, decimal salario)
        {
            try
            {
                // Validar campos obligatorios
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(Correo_Electronico) || string.IsNullOrEmpty(departamento) || string.IsNullOrEmpty(cargo))
                {
                    ViewBag.Message = "Por favor complete todos los campos obligatorios.";
                    return View("Index");
                }

                // Validar formato de correo electrónico
                if (!Correo_Electronico.Contains("@"))
                {
                    ViewBag.Message = "El correo electrónico debe tener un formato válido.";
                    return View("Index");
                }

                // Validar edad
                if (edad < 18)
                {
                    ViewBag.Message = "La edad debe ser mayor o igual a 18 años.";
                    return View("Index");
                }

                // Establecer la cadena de conexión a la base de datos
                string connectionString = "Server=(localdb)\\senati;Database=web;Integrated Security=true;";

                // Definir la consulta SQL para insertar el empleado
                string query = @"INSERT INTO Empleado (Nombre, Apellido, Edad, Genero, Correo_Electronico, Departamento, Cargo, Salario) 
                     VALUES (@Nombre, @Apellido, @Edad, @Genero, @Correo, @Departamento, @Cargo, @Salario)";

                // Crear y abrir la conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear el comando SQL con la consulta y la conexión
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Asignar los parámetros a la consulta SQL
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Apellido", apellido);
                        command.Parameters.AddWithValue("@Edad", edad);
                        command.Parameters.AddWithValue("@Genero", genero);
                        command.Parameters.AddWithValue("@Correo", Correo_Electronico);
                        command.Parameters.AddWithValue("@Departamento", departamento);
                        command.Parameters.AddWithValue("@Cargo", cargo);
                        command.Parameters.AddWithValue("@Salario", salario);

                        // Ejecutar la consulta SQL para insertar el empleado
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verificar si se insertó correctamente el empleado
                        if (rowsAffected > 0)
                        {
                            ViewBag.Message = "Empleado agregado correctamente.";
                        }
                        else
                        {
                            ViewBag.Message = "Error al agregar el empleado.";
                        }
                    }
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Se produjo un error al procesar la solicitud: " + ex.Message);
                return View("Index");
            }
        }

        public IActionResult Privacy()
        {
            DataTable empleados = ObtenerEmpleadosDesdeBD();
            return View(empleados);
        }

        public IActionResult Empleados()
        {
            DataTable empleados = ObtenerEmpleadosDesdeBD();
            return View(empleados);
        }

        private DataTable ObtenerEmpleadosDesdeBD()
        {
            DataTable empleados = new DataTable();

            string connectionString = "Server=(localdb)\\senati;Database=web;Integrated Security=true;";
            string query = "SELECT Nombre, Apellido, Departamento FROM Empleado";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(empleados);
                }
            }

            return empleados;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
