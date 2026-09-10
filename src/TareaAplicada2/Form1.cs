using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TareaAplicada2
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient;
        private readonly ClimaService _climaService;
        private readonly FavoritosService _favoritosService;
        private DataTable _tablaConsultas;
        private ConsultaFavorita _ultimaConsulta;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _climaService = new ClimaService(_httpClient);
            _favoritosService = new FavoritosService("favoritos.csv");
            InicializarTabla();
            CargarFavoritosAlIniciar();
        }

        private void InicializarTabla()
        {
            _tablaConsultas = new DataTable();
            _tablaConsultas.Columns.Add("Ciudad", typeof(string));
            _tablaConsultas.Columns.Add("Temperatura (°C)", typeof(string));
            _tablaConsultas.Columns.Add("Condición", typeof(string));
            _tablaConsultas.Columns.Add("Fecha / Hora", typeof(string));

            dgvResultados.DataSource = _tablaConsultas;
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.MultiSelect = false;
        }

        private void CargarFavoritosAlIniciar()
        {
            try
            {
                ActualizarEstado("Cargando favoritos guardados...", Color.DarkBlue);
                List<ConsultaFavorita> favoritos = _favoritosService.CargarFavoritos();

                foreach (var fav in favoritos)
                {
                    _tablaConsultas.Rows.Add(
                        fav.Ciudad,
                        $"{fav.TemperaturaC:F1}°C",
                        fav.Condicion,
                        fav.FechaConsulta.ToString("g")
                    );
                }

                ActualizarEstado($"Listo. Se cargaron {favoritos.Count} favorito(s) guardado(s).", Color.DarkGreen);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(
                    $"No se pudieron cargar todos los favoritos previos:\n{ex.Message}",
                    "Aviso al iniciar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ActualizarEstado("Listo (sin favoritos previos)", Color.DarkOrange);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado al leer el archivo local: {ex.Message}",
                    "Error de inicio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Listo con errores locales", Color.DarkRed);
            }
        }

        private async void btnConsultar_Click(object sender, EventArgs e)
        {
            string ciudad = txtCiudad.Text.Trim();

            if (string.IsNullOrWhiteSpace(ciudad))
            {
                MessageBox.Show(
                    "Por favor ingrese el nombre de una ciudad para consultar el clima.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtCiudad.Focus();
                return;
            }

            if (ciudad.Length < 2)
            {
                MessageBox.Show(
                    "El nombre de la ciudad debe contener al menos 2 caracteres.",
                    "Validación de entrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                txtCiudad.SelectAll();
                txtCiudad.Focus();
                return;
            }

            try
            {
                BloquearControlesDurantePeticion(true);
                ActualizarEstado($"Consultando clima de '{ciudad}' en tiempo real...", Color.DarkBlue);

                string jsonRespuesta = await _climaService.ObtenerClimaAsync(ciudad);
                ConsultaFavorita consulta = _favoritosService.ConvertirDesdeJson(jsonRespuesta);
                _ultimaConsulta = consulta;

                _tablaConsultas.Rows.Add(
                    consulta.Ciudad,
                    $"{consulta.TemperaturaC:F1}°C",
                    consulta.Condicion,
                    consulta.FechaConsulta.ToString("g")
                );

                if (dgvResultados.Rows.Count > 0)
                {
                    dgvResultados.ClearSelection();
                    dgvResultados.Rows[dgvResultados.Rows.Count - 1].Selected = true;
                }

                btnFavorito.Enabled = true;
                ActualizarEstado($"Clima obtenido con éxito para '{consulta.Ciudad}': {consulta.TemperaturaC}°C, {consulta.Condicion}", Color.DarkGreen);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarEstado("Error de validación", Color.DarkOrange);
            }
            catch (HttpRequestException ex)
            {
                string mensajeUsuario = "No se pudo conectar con el servicio de clima.\n\n" +
                                        "Posibles causas:\n" +
                                        "• No hay conexión a Internet.\n" +
                                        "• La ciudad ingresada no fue encontrada en la API.\n" +
                                        "• El servicio de WeatherAPI está temporalmente fuera de línea.\n\n" +
                                        $"Detalle técnico: {ex.Message}";

                MessageBox.Show(mensajeUsuario, "Error de conexión o consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstado("Error de conexión con la API", Color.DarkRed);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show($"Error al procesar los datos de la respuesta:\n{ex.Message}", "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstado("Error en el formato de datos", Color.DarkRed);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado:\n{ex.Message}", "Error general", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstado("Error general en la consulta", Color.DarkRed);
            }
            finally
            {
                BloquearControlesDurantePeticion(false);
            }
        }

        private void btnFavorito_Click(object sender, EventArgs e)
        {
            if (_ultimaConsulta == null)
            {
                MessageBox.Show("Primero debe realizar una consulta de clima exitosa para poder guardarla como favorita.", "Sin consulta previa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                DialogResult confirmacion = MessageBox.Show(
                    $"¿Desea guardar '{_ultimaConsulta.Ciudad}' ({_ultimaConsulta.TemperaturaC}°C, {_ultimaConsulta.Condicion}) en su lista permanente de favoritos?",
                    "Confirmar guardado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                );

                if (confirmacion != DialogResult.Yes)
                {
                    ActualizarEstado("Guardado cancelado por el usuario.", Color.DarkSlateGray);
                    return;
                }

                _favoritosService.GuardarFavorito(_ultimaConsulta);
                MessageBox.Show($"¡La consulta de '{_ultimaConsulta.Ciudad}' se guardó exitosamente en favoritos!", "Guardado exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActualizarEstado($"Favorito guardado: {_ultimaConsulta.Ciudad}", Color.DarkGreen);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show($"No se pudo guardar el favorito:\n{ex.Message}\n\nVerifique que 'favoritos.csv' no esté abierto en Excel.", "Error de persistencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstado("Error al guardar archivo CSV", Color.DarkRed);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarEstado("Error inesperado", Color.DarkRed);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor seleccione la fila que desea eliminar en la tabla.", "Ningún elemento seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow fila = dgvResultados.SelectedRows[0];
            string ciudad = fila.Cells["Ciudad"].Value?.ToString() ?? "desconocida";

            DialogResult respuesta = MessageBox.Show(
                $"¿Está completamente seguro de que desea eliminar el registro de '{ciudad}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    _tablaConsultas.Rows.RemoveAt(fila.Index);
                    ActualizarEstado($"Registro de '{ciudad}' eliminado.", Color.DarkOrange);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo eliminar la fila: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BloquearControlesDurantePeticion(bool bloqueado)
        {
            btnConsultar.Enabled = !bloqueado;
            txtCiudad.Enabled = !bloqueado;
            Cursor = bloqueado ? Cursors.WaitCursor : Cursors.Default;
        }

        private void ActualizarEstado(string mensaje, Color color)
        {
            lblEstado.Text = $"Estado: {mensaje}";
            lblEstado.ForeColor = color;
        }

        private void txtCiudad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConsultar.PerformClick();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _httpClient?.Dispose();
        }
    }
}
