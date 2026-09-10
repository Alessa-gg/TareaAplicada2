namespace TareaAplicada2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblCiudad = new Label();
            this.txtCiudad = new TextBox();
            this.btnConsultar = new Button();
            this.btnFavorito = new Button();
            this.lblEstado = new Label();
            this.dgvResultados = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultados)).BeginInit();
            this.SuspendLayout();

            // lblCiudad
            this.lblCiudad.AutoSize = true;
            this.lblCiudad.Location = new Point(20, 20);
            this.lblCiudad.Name = "lblCiudad";
            this.lblCiudad.Size = new Size(45, 15);
            this.lblCiudad.Text = "Ciudad:";

            // txtCiudad
            this.txtCiudad.Location = new Point(80, 17);
            this.txtCiudad.Name = "txtCiudad";
            this.txtCiudad.Size = new Size(200, 23);
            this.txtCiudad.PlaceholderText = "Ej. San Salvador";

            // btnConsultar
            this.btnConsultar.Location = new Point(300, 16);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new Size(100, 25);
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.UseVisualStyleBackColor = true;
            this.btnConsultar.Click += new EventHandler(this.btnConsultar_Click);

            // btnFavorito
            this.btnFavorito.Location = new Point(410, 16);
            this.btnFavorito.Name = "btnFavorito";
            this.btnFavorito.Size = new Size(120, 25);
            this.btnFavorito.Text = "Guardar Favorito";
            this.btnFavorito.UseVisualStyleBackColor = true;
            this.btnFavorito.Click += new EventHandler(this.btnFavorito_Click);

            // lblEstado
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new Point(20, 55);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new Size(100, 15);
            this.lblEstado.Text = "Estado: listo";

            // dgvResultados
            this.dgvResultados.Location = new Point(20, 85);
            this.dgvResultados.Name = "dgvResultados";
            this.dgvResultados.Size = new Size(510, 250);
            this.dgvResultados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResultados.AllowUserToAddRows = false;
            this.dgvResultados.ReadOnly = true;

            // Form1
            this.ClientSize = new Size(550, 360);
            this.Controls.Add(this.lblCiudad);
            this.Controls.Add(this.txtCiudad);
            this.Controls.Add(this.btnConsultar);
            this.Controls.Add(this.btnFavorito);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.dgvResultados);
            this.Name = "Form1";
            this.Text = "App Clima - Punto 3 (Diseño de Interfaz)";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblCiudad;
        private TextBox txtCiudad;
        private Button btnConsultar;
        private Button btnFavorito;
        private Label lblEstado;
        private DataGridView dgvResultados;
    }
}
