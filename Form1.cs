using System.Data;

namespace AppClima
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConfigurarTabla();
        }

        private void ConfigurarTabla()
        {
            DataTable tabla = new DataTable();
            tabla.Columns.Add("Ciudad");
            tabla.Columns.Add("Temperatura");
            tabla.Columns.Add("Condicion");
            dgvResultados.DataSource = tabla;
        }

        // --- Punto 3: manejador del botón, usa async/await ---
        private async void btnConsultar_Click(object sender, EventArgs e)
        {
            string ciudad = txtCiudad.Text.Trim();

            if (string.IsNullOrEmpty(ciudad))
            {
                MessageBox.Show("Por favor escribe una ciudad.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                lblEstado.Text = "Estado: consultando...";
                btnConsultar.Enabled = false;

                // TODO: cuando el compañero del Punto 2 suba ClimaService.cs,
                // reemplazar esta línea por la llamada real, por ejemplo:
                // var datos = await ClimaService.ObtenerClimaAsync(ciudad);
                var datos = await ObtenerClimaDummyAsync(ciudad);

                ((DataTable)dgvResultados.DataSource!).Rows.Add(
                    datos.Ciudad, datos.Temperatura, datos.Condicion);

                lblEstado.Text = "Estado: consulta exitosa";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al consultar el clima: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblEstado.Text = "Estado: error";
            }
            finally
            {
                btnConsultar.Enabled = true;
            }
        }

        private void btnFavorito_Click(object sender, EventArgs e)
        {
            // TODO: integrar con FavoritosService.cs (Punto 4) para
            // guardar/cargar el CSV de favoritos.
            MessageBox.Show("Aquí se integrará el guardado de favoritos (Punto 4).",
                "Pendiente de integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- Método dummy que simula una llamada asíncrona a una API ---
        // Sirve para probar la interfaz sin depender del Punto 2.
        private async Task<(string Ciudad, string Temperatura, string Condicion)> ObtenerClimaDummyAsync(string ciudad)
        {
            await Task.Delay(1500); // Simula la latencia de una petición HTTP real

            var random = new Random();
            string[] condiciones = { "Soleado", "Nublado", "Lluvioso", "Parcialmente nublado" };
            int temperatura = random.Next(18, 32);
            string condicion = condiciones[random.Next(condiciones.Length)];

            return (ciudad, $"{temperatura}°C", condicion);
        }
    }
}
