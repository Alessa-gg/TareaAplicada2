using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TareaAplicada2
{
    public class ClimaService
    {
        private readonly HttpClient _httpClient;

        // Reutilizar la instancia de HttpClient para evitar el agotamiento de sockets
        public ClimaService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Consulta el clima de una ciudad mediante una llamada HTTP asíncrona.
        /// </summary>
        public async Task<string> ObtenerClimaAsync(string ciudad)
        {
            if (string.IsNullOrWhiteSpace(ciudad))
                throw new ArgumentException("El nombre de la ciudad no puede estar vacío.", nameof(ciudad));

            string apiKey = "7b440475765c43c9be601736261009";
            string url = $"https://weatherapi.com{apiKey}&q={Uri.EscapeDataString(ciudad)}&lang=es";

            // Llamada asíncrona para mantener la interfaz WinForms fluida
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
