using Microsoft.AspNetCore.Mvc;
using appweb_MBC.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;

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

                // Validar formato de correo electr�nico
                if (!Correo_Electronico.Contains("@"))
                {
                    ViewBag.Message = "El correo electr�nico debe tener un formato v�lido.";
                    return View("Index");
                }

                // Validar edad
                if (edad < 18)
                {
                    ViewBag.Message = "La edad debe ser mayor o igual a 18 a�os.";
                    return View("Index");
                }

                // Establecer la cadena de conexi�n a la base de datos
                string connectionString = "Server=DESKTOP-HONFTB2;Database=web;Integrated Security=true;";

                // Definir la consulta SQL para insertar el empleado
                string query = @"INSERT INTO Empleado (Nombre, Apellido, Edad, Genero, Correo_Electronico, Departamento, Cargo, Salario) 
                     VALUES (@Nombre, @Apellido, @Edad, @Genero, @Correo, @Departamento, @Cargo, @Salario)";

                // Crear y abrir la conexi�n a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear el comando SQL con la consulta y la conexi�n
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Asignar los par�metros a la consulta SQL
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

                        // Verificar si se insert� correctamente el empleado
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

        [HttpPost]
        public IActionResult UpdateEmployee(string nombre, string apellido, int edad, string genero, string correo, string cargo, decimal salario)
        {
            try
            {
                // Establecer la cadena de conexi�n a la base de datos
                string connectionString = "Server=DESKTOP-HONFTB2;Database=web;Integrated Security=true;";

                // Definir la consulta SQL para actualizar el empleado
                string query = @"UPDATE Empleado SET Nombre = @Nombre, Apellido = @Apellido, Edad = @Edad, Genero = @Genero, Correo_Electronico = @Correo, Cargo = @Cargo, Salario = @Salario WHERE Correo_Electronico = @Correo";

                // Crear y abrir la conexi�n a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear el comando SQL con la consulta y la conexi�n
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Asignar los par�metros a la consulta SQL
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@Apellido", apellido);
                        command.Parameters.AddWithValue("@Edad", edad);
                        command.Parameters.AddWithValue("@Genero", genero);
                        command.Parameters.AddWithValue("@Correo", correo);
                        command.Parameters.AddWithValue("@Cargo", cargo);
                        command.Parameters.AddWithValue("@Salario", salario);

                        // Ejecutar la consulta SQL para actualizar el empleado
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verificar si se actualiz� correctamente el empleado
                        if (rowsAffected > 0)
                        {
                            ViewBag.Message = "Empleado actualizado correctamente.";
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar el empleado.";
                        }
                    }
                }

                return RedirectToAction("Privacy");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Se produjo un error al procesar la solicitud: " + ex.Message);
                return RedirectToAction("Privacy");
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
            string jsonEmpleados = JsonSerializer.Serialize(empleados);
            return Content(jsonEmpleados, "application/json");
        }

        private DataTable ObtenerEmpleadosDesdeBD()
        {
            DataTable empleados = new DataTable();

            string connectionString = "Server=DESKTOP-HONFTB2;Database=web;Integrated Security=true;";
            string query = "SELECT * FROM Empleado"; // Seleccionar todos los campos de la tabla Empleado

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

        [HttpPost]
        public IActionResult Privacy(string correo)
        {
            try
            {
                // Establecer la cadena de conexi�n a la base de datos
                string connectionString = "Server=DESKTOP-HONFTB2;Database=web;Integrated Security=true;";

                // Definir la consulta SQL para eliminar al empleado por su correo electr�nico
                string query = @"DELETE FROM Empleado WHERE Correo_Electronico = @Correo";

                // Crear y abrir la conexi�n a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear el comando SQL con la consulta y la conexi�n
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Asignar el par�metro Correo_Electronico a la consulta SQL
                        command.Parameters.AddWithValue("@Correo", correo);

                        // Ejecutar la consulta SQL para eliminar al empleado
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verificar si se elimin� correctamente al empleado
                        if (rowsAffected > 0)
                        {
                            ViewBag.Message = "Empleado eliminado correctamente.";
                        }
                        else
                        {
                            ViewBag.Message = "No se encontr� al empleado con el correo proporcionado.";
                        }
                    }
                }

                // Redirigir a la acci�n que muestra la lista de empleados actualizada
                return RedirectToAction("Privacy");
            }
            catch (Exception ex)
            {
                // Manejar el error si ocurre alg�n problema al eliminar al empleado
                ModelState.AddModelError(string.Empty, "Se produjo un error al eliminar al empleado: " + ex.Message);
                return RedirectToAction("Privacy");
            }
        }


    }
}
