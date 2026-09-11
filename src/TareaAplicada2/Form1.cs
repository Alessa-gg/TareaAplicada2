using System.ComponentModel;
using System.Net.Http;

namespace TareaAplicada2
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ClimaService _climaService;
        private readonly FavoritosService _favoritosService;
        private readonly BindingList<ConsultaFavorita> _favoritos;

        // Guarda la última consulta hecha (aún no guardada como favorito)
        private ConsultaFavorita? _consultaActual;

        public Form1()
        {
            InitializeComponent();

            _climaService = new ClimaService(_httpClient);
            _favoritosService = new FavoritosService();

            _favoritos = new BindingList<ConsultaFavorita>(_favoritosService.CargarFavoritos());
            dgvResultados.DataSource = _favoritos;
        }

        private async void btnConsultar_Click(object sender, EventArgs e)
        {
            string ciudad = txtCiudad.Text.Trim();

            if (string.IsNullOrWhiteSpace(ciudad))
            {
                MessageBox.Show("Por favor escribe el nombre de una ciudad.", "Dato requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnConsultar.Enabled = false;
            btnFavorito.Enabled = false;
            lblEstado.Text = "Estado: Buscando clima...";

            try
            {
                string json = await _climaService.ObtenerClimaAsync(ciudad);
                _consultaActual = _favoritosService.ConvertirDesdeJson(json);

                lblEstado.Text = $"Estado: {_consultaActual}";
                btnFavorito.Enabled = true;
            }
            catch (Exception ex)
            {
                _consultaActual = null;
                lblEstado.Text = "Estado: error en la consulta";
                MessageBox.Show($"Ocurrió un error al consultar el clima: {ex.Message}",
                    "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConsultar.Enabled = true;
            }
        }

        private void btnFavorito_Click(object sender, EventArgs e)
        {
            if (_consultaActual == null)
            {
                MessageBox.Show("Primero consulta el clima de una ciudad.", "Nada que guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _favoritosService.GuardarFavorito(_consultaActual);
                _favoritos.Add(_consultaActual);
                lblEstado.Text = "Estado: favorito guardado correctamente";
                btnFavorito.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo guardar el favorito: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.CurrentRow?.DataBoundItem is not ConsultaFavorita seleccionado)
            {
                MessageBox.Show("Selecciona una fila de la tabla para eliminar.", "Nada seleccionado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _favoritosService.EliminarFavorito(seleccionado);
                _favoritos.Remove(seleccionado);
                lblEstado.Text = "Estado: favorito eliminado";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo eliminar el favorito: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCiudad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnConsultar_Click(sender, e);
            }
        }
    }
}
