using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TareaAplicada2
{
    // Esta clase se encarga de guardar y cargar los favoritos en un archivo CSV local
    // Con esto la app se acuerda de los favoritos aunque no haya internet o se cierre el programa
    public class FavoritosService
    {
        private readonly string rutaArchivo;
        private const char Separador = ',';
        private const string Encabezado = "Ciudad,TemperaturaC,Condicion,FechaConsulta";

        public FavoritosService(string rutaArchivo = "favoritos.csv")
        {
            this.rutaArchivo = rutaArchivo;
        }

        // ClimaService.ObtenerClimaAsync me devuelve el JSON en crudo (un string),
        // así que aquí lo convierto a mi modelo ConsultaFavorita para poder trabajarlo
        // el JSON de weatherapi.com viene mas o menos así:
        // { "location": { "name": "San Salvador" },
        //   "current": { "temp_c": 27.0, "condition": { "text": "Soleado" } } }
        public ConsultaFavorita ConvertirDesdeJson(string jsonClima)
        {
            if (string.IsNullOrWhiteSpace(jsonClima))
                throw new ArgumentException("La respuesta de la API está vacía.", nameof(jsonClima));

            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonClima);
                JsonElement root = doc.RootElement;

                // saco los datos que me interesan del JSON
                string ciudad = root.GetProperty("location").GetProperty("name").GetString();
                JsonElement current = root.GetProperty("current");
                double temperatura = current.GetProperty("temp_c").GetDouble();
                string condicion = current.GetProperty("condition").GetProperty("text").GetString();

                return new ConsultaFavorita
                {
                    Ciudad = ciudad,
                    TemperaturaC = temperatura,
                    Condicion = condicion,
                    FechaConsulta = DateTime.Now
                };
            }
            catch (JsonException ex)
            {
                // si el JSON viene mal formado o cambia la estructura, no truena la app
                throw new ApplicationException("La respuesta de la API no tiene el formato esperado.", ex);
            }
            catch (KeyNotFoundException ex)
            {
                throw new ApplicationException("Faltan datos en la respuesta de la API.", ex);
            }
        }

        // Guarda un favorito nuevo al final del archivo CSV
        public void GuardarFavorito(ConsultaFavorita favorito)
        {
            if (favorito == null)
                throw new ArgumentNullException(nameof(favorito), "La consulta a guardar no puede ser nula.");

            try
            {
                bool existeArchivo = File.Exists(rutaArchivo);

                using (StreamWriter writer = new StreamWriter(rutaArchivo, append: true))
                {
                    // si es la primera vez que se guarda algo, primero pongo el encabezado
                    if (!existeArchivo)
                    {
                        writer.WriteLine(Encabezado);
                    }

                    writer.WriteLine(ConvertirALinea(favorito));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ApplicationException("No hay permisos para escribir el archivo de favoritos.", ex);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("No se pudo guardar el favorito en el archivo local.", ex);
            }
        }

        // Lee el CSV y regresa la lista de favoritos que ya estaban guardados
        // Si todavía no existe el archivo (primera vez que se abre la app), regreso lista vacía
        public List<ConsultaFavorita> CargarFavoritos()
        {
            List<ConsultaFavorita> favoritos = new List<ConsultaFavorita>();

            if (!File.Exists(rutaArchivo))
            {
                return favoritos;
            }

            try
            {
                string[] lineas = File.ReadAllLines(rutaArchivo);

                foreach (string linea in lineas.Skip(1)) // me salto la línea del encabezado
                {
                    if (string.IsNullOrWhiteSpace(linea)) continue;

                    ConsultaFavorita favorito = ConvertirDesdeLinea(linea);
                    if (favorito != null)
                    {
                        favoritos.Add(favorito);
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ApplicationException("No se pudo leer el archivo de favoritos.", ex);
            }

            return favoritos;
        }

        // Borra un favorito específico y vuelve a escribir todo el archivo sin ese
        public void EliminarFavorito(ConsultaFavorita favorito)
        {
            List<ConsultaFavorita> favoritos = CargarFavoritos();
            favoritos.RemoveAll(f => f.Ciudad == favorito.Ciudad && f.FechaConsulta == favorito.FechaConsulta);

            try
            {
                using (StreamWriter writer = new StreamWriter(rutaArchivo, append: false))
                {
                    writer.WriteLine(Encabezado);
                    foreach (ConsultaFavorita f in favoritos)
                    {
                        writer.WriteLine(ConvertirALinea(f));
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ApplicationException("No se pudo actualizar el archivo de favoritos.", ex);
            }
        }

        // convierte un ConsultaFavorita a una línea de texto para el CSV
        private string ConvertirALinea(ConsultaFavorita favorito)
        {
            return string.Join(Separador.ToString(), new[]
            {
                favorito.Ciudad,
                favorito.TemperaturaC.ToString(CultureInfo.InvariantCulture),
                favorito.Condicion,
                favorito.FechaConsulta.ToString("o", CultureInfo.InvariantCulture)
            });
        }

        // hace lo contrario, de una línea del CSV arma el objeto
        private ConsultaFavorita ConvertirDesdeLinea(string linea)
        {
            string[] campos = linea.Split(Separador);

            // si la línea viene incompleta o rara, mejor la ignoro que tronar la app
            if (campos.Length < 4) return null;

            try
            {
                return new ConsultaFavorita
                {
                    Ciudad = campos[0],
                    TemperaturaC = double.Parse(campos[1], CultureInfo.InvariantCulture),
                    Condicion = campos[2],
                    FechaConsulta = DateTime.Parse(campos[3], CultureInfo.InvariantCulture)
                };
            }
            catch (FormatException)
            {
                return null; // dato mal formado, se ignora esa línea
            }
        }
    }
}
