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
    /// <summary>
    /// Formulario principal de la aplicación de clima.
    /// Implementa el Manejo Robusto de Excepciones y la Interacción con el Usuario (Punto 5).
    /// </summary>
    public partial class Form1 : Form
    {
        // Servicios inyectados y reutilizados (Buenas prácticas de consumo HTTP y persistencia)
        private readonly HttpClient _httpClient;
        private readonly ClimaService _climaService;
        private readonly FavoritosService _favoritosService;

        // Estructura de datos en memoria para enlazar al DataGridView
        private DataTable _tablaConsultas;
        private ConsultaFavorita _ultimaConsulta;

        public Form1()
        {
            InitializeComponent();

            // 1. Inicialización de servicios
            _httpClient = new HttpClient();
            _climaService = new ClimaService(_httpClient);
            _favoritosService = new FavoritosService("favoritos.csv");

            // 2. Configurar la tabla de la interfaz gráfica
            InicializarTabla();

            // 3. Cargar automáticamente favoritos al iniciar con control de excepciones
            CargarFavoritosAlIniciar();
        }

        /// <summary>
        /// Configura las columnas del DataGridView para mostrar los resultados de forma ordenada.
        /// </summary>
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

        /// <summary>
        /// Carga los favoritos guardados previamente en el archivo CSV local.
        /// Protegido con bloque try-catch para no impedir el arranque si el archivo está dañado o bloqueado.
        /// </summary>
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
                // Notificación amigable de advertencia sin interrumpir el arranque del programa
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

        // ====================================================================
        // MANEJO DEL BOTÓN CONSULTAR CLIMA (try-catch jerárquico asíncrono)
        // ====================================================================
        private async void btnConsultar_Click(object sender, EventArgs e)
        {
            // 1. VALIDACIÓN PREVENTIVA DE ENTRADA (Input Validation)
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

            // 2. BLOQUE TRY-CATCH-FINALLY ASÍNCRONO
            try
            {
                // Cambiar estado visual para prevenir doble clic y dar feedback al usuario
                BloquearControlesDurantePeticion(true);
                ActualizarEstado($"Consultando clima de '{ciudad}' en tiempo real...", Color.DarkBlue);

                // Llamada HTTP asíncrona a ClimaService (Punto 2)
                string jsonRespuesta = await _climaService.ObtenerClimaAsync(ciudad);

                // Conversión de JSON a modelo C# mediante FavoritosService (Punto 4)
                ConsultaFavorita consulta = _favoritosService.ConvertirDesdeJson(jsonRespuesta);
                _ultimaConsulta = consulta;

                // Agregar los datos obtenidos a la tabla
                _tablaConsultas.Rows.Add(
                    consulta.Ciudad,
                    $"{consulta.TemperaturaC:F1}°C",
                    consulta.Condicion,
                    consulta.FechaConsulta.ToString("g")
                );

                // Seleccionar la última fila agregada
                if (dgvResultados.Rows.Count > 0)
                {
                    dgvResultados.ClearSelection();
                    dgvResultados.Rows[dgvResultados.Rows.Count - 1].Selected = true;
                }

                // Habilitar botón de guardar favorito
                btnFavorito.Enabled = true;

                // Notificación de éxito en la interfaz
                ActualizarEstado($"Clima obtenido con éxito para '{consulta.Ciudad}': {consulta.TemperaturaC}°C, {consulta.Condicion}", Color.DarkGreen);
            }
            // Captura de argumentos inválidos
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Datos inválidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                ActualizarEstado("Error de validación", Color.DarkOrange);
            }
            // Captura de errores de red, DNS o HTTP (404, 500, sin internet)
            catch (HttpRequestException ex)
            {
                string mensajeUsuario = "No se pudo conectar con el servicio de clima.\n\n" +
                                        "Posibles causas:\n" +
                                        "• No hay conexión a Internet.\n" +
                                        "• La ciudad ingresada no fue encontrada en la API.\n" +
                                        "• El servicio de WeatherAPI está temporalmente fuera de línea.\n\n" +
                                        $"Detalle técnico: {ex.Message}";

                MessageBox.Show(
                    mensajeUsuario,
                    "Error de conexión o consulta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Error de conexión con la API", Color.DarkRed);
            }
            // Captura de errores en el formato JSON de la respuesta
            catch (ApplicationException ex)
            {
                MessageBox.Show(
                    $"Error al procesar los datos de la respuesta:\n{ex.Message}",
                    "Error de formato",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Error en el formato de datos", Color.DarkRed);
            }
            // Captura de cualquier excepción imprevista para evitar que la aplicación se cierre (Crash)
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió un error inesperado:\n{ex.Message}",
                    "Error general",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Error general en la consulta", Color.DarkRed);
            }
            finally
            {
                // El bloque finally SIEMPRE se ejecuta: restauramos los controles y el cursor
                BloquearControlesDurantePeticion(false);
            }
        }

        // ====================================================================
        // MANEJO DEL BOTÓN GUARDAR FAVORITO (con diálogos de confirmación)
        // ====================================================================
        private void btnFavorito_Click(object sender, EventArgs e)
        {
            // Validar que exista una consulta lista para guardar
            if (_ultimaConsulta == null)
            {
                MessageBox.Show(
                    "Primero debe realizar una consulta de clima exitosa para poder guardarla como favorita.",
                    "Sin consulta previa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            try
            {
                // DIÁLOGO DE CONFIRMACIÓN AL USUARIO (Interacción segura)
                DialogResult confirmacion = MessageBox.Show(
                    $"¿Desea guardar '{_ultimaConsulta.Ciudad}' ({_ultimaConsulta.TemperaturaC}°C, {_ultimaConsulta.Condicion}) en su lista permanente de favoritos?",
                    "Confirmar guardado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                );

                if (confirmacion != DialogResult.Yes)
                {
                    ActualizarEstado("Guardado de favorito cancelado por el usuario.", Color.DarkSlateGray);
                    return;
                }

                // Guardar en el archivo CSV local mediante FavoritosService (Punto 4)
                _favoritosService.GuardarFavorito(_ultimaConsulta);

                MessageBox.Show(
                    $"¡La consulta de '{_ultimaConsulta.Ciudad}' se guardó exitosamente en el archivo local de favoritos!",
                    "Guardado exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ActualizarEstado($"Favorito guardado: {_ultimaConsulta.Ciudad}", Color.DarkGreen);
            }
            // Excepción de permisos o archivo en uso (ej. abierto en Microsoft Excel)
            catch (ApplicationException ex)
            {
                MessageBox.Show(
                    $"No se pudo guardar el favorito localmente:\n{ex.Message}\n\n" +
                    "Sugerencia: Verifique que el archivo 'favoritos.csv' no esté abierto en otra aplicación como Excel.",
                    "Error de persistencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Error al guardar archivo CSV", Color.DarkRed);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado al guardar favorito: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ActualizarEstado("Error inesperado en favoritos", Color.DarkRed);
            }
        }

        // ====================================================================
        // ELIMINAR REGISTRO SELECCIONADO (Interacción con usuario y confirmación)
        // ====================================================================
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Por favor seleccione la fila que desea eliminar en la tabla.",
                    "Ningún elemento seleccionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DataGridViewRow fila = dgvResultados.SelectedRows[0];
            string ciudad = fila.Cells["Ciudad"].Value?.ToString() ?? "desconocida";

            // Diálogo de confirmación con icono de advertencia
            DialogResult respuesta = MessageBox.Show(
                $"¿Está completamente seguro de que desea eliminar el registro de '{ciudad}' de la lista?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    int index = fila.Index;
                    _tablaConsultas.Rows.RemoveAt(index);
                    ActualizarEstado($"Registro de '{ciudad}' eliminado de la vista.", Color.DarkOrange);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"No se pudo eliminar la fila: {ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // ====================================================================
        // MÉTODOS AUXILIARES DE INTERACCIÓN Y ESTADO DE LA INTERFAZ
        // ====================================================================

        /// <summary>
        /// Bloquea temporalmente los controles mientras se ejecuta una tarea asíncrona
        /// para garantizar consistencia y evitar peticiones concurrentes accidentales.
        /// </summary>
        private void BloquearControlesDurantePeticion(bool bloqueado)
        {
            btnConsultar.Enabled = !bloqueado;
            txtCiudad.Enabled = !bloqueado;
            Cursor = bloqueado ? Cursors.WaitCursor : Cursors.Default;
        }

        /// <summary>
        /// Actualiza la barra de estado con un texto descriptivo y color temático.
        /// </summary>
        private void ActualizarEstado(string mensaje, Color color)
        {
            lblEstado.Text = $"Estado: {mensaje}";
            lblEstado.ForeColor = color;
        }

        /// <summary>
        /// Permite realizar la consulta presionando la tecla ENTER en el TextBox.
        /// </summary>
        private void txtCiudad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evitar sonido beep de Windows
                btnConsultar.PerformClick();
            }
        }

        /// <summary>
        /// Limpieza segura de recursos no administrados al cerrar el formulario.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _httpClient?.Dispose();
        }
    }
}
