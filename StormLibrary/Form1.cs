using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StormLibrary
{
    public partial class Form1 : Form
    {
        UpdateManager updateManager = new UpdateManager();
        List<Juego> juegos;
        private System.Windows.Forms.Timer checkFileTimer;
        private Juego juegoSeleccionado;
        private Button btnAccion;

        private readonly string downloadsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StormGamesStudios",
            "StormLibraryV2",
            "downloads"
        );

        private readonly string dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StormGamesStudios",
            "StormLibraryV2",
            "gamesCheck"
        );

        public Form1()
        {
            InitializeComponent();

            checkFileTimer = new System.Windows.Forms.Timer();
            checkFileTimer.Interval = 2000;
            checkFileTimer.Tick += CheckFileTimer_Tick;

            Directory.CreateDirectory(downloadsDir);
            Directory.CreateDirectory(dataDir);

            MinimumSize = new Size(878, 547);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "Actualizando...";
            await updateManager.CheckAndDownloadFiles(dataDir);
            juegos = await updateManager.LoadGames(dataDir);

            listGames.DataSource = juegos;
            listGames.DisplayMember = "nombre";
            labelStatus.Text = "Actualizado";
        }

        private void listGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listGames.SelectedItem == null) return;
            juegoSeleccionado = (Juego)listGames.SelectedItem;
            MostrarJuegoEnPanel(juegoSeleccionado);
            checkFileTimer.Start();
        }

        private async void MostrarJuegoEnPanel(Juego juego)
        {
            panelJuego.Controls.Clear();

            string carpetaJuego = Path.GetFullPath(juego.ubicacion.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
            string rutaEjecutable = Path.Combine(carpetaJuego, juego.archivo_ejecutable);

            // Logo
            PictureBox logo = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(20, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            string logoPath = Path.Combine(updateManager.LogosDir, juego.nombre.Replace(" ", "") + ".png");

            // Descargar logo si no existe
            if (!File.Exists(logoPath) && !string.IsNullOrEmpty(juego.logo))
            {
                try
                {
                    using (HttpClient http = new HttpClient())
                    {
                        byte[] data = await http.GetByteArrayAsync(juego.logo);
                        Directory.CreateDirectory(updateManager.LogosDir);
                        File.WriteAllBytes(logoPath, data);
                    }
                }
                catch
                {
                    // Ignorar errores de descarga
                }
            }

            if (File.Exists(logoPath))
                logo.ImageLocation = logoPath;

            // Título
            Label lblRequisitos = new Label
            {
                Text = "Requisitos:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(180, Bottom + 10),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Descripción
            TextBox desc = new TextBox
            {
                Text = juego.descripcion,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(180, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Width = panelJuego.Width - 200,
                Height = 150,
                BackColor = panelJuego.BackColor,
                ForeColor = panelJuego.ForeColor
            };

            TextBox txtRequisitos = new TextBox
            {
                Text = juego.requisitos ?? "No especificados",
                Multiline = true,
                ReadOnly = true,
                Location = new Point(180, lblRequisitos.Bottom + 5),
                Width = panelJuego.Width - 200,
                Height = 60,
                BackColor = panelJuego.BackColor,
                ForeColor = panelJuego.ForeColor,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Botón Abrir / Descargar
            btnAccion = new Button
            {
                Size = new Size(200, 40),
                Location = new Point(180, 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Botón Abrir carpeta
            Button btnAbrirCarpeta = new Button
            {
                Size = new Size(200, 40),
                Location = new Point(400, 220),
                Text = "Abrir carpeta",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnAbrirCarpeta.Click += (s, e) =>
            {
                if (Directory.Exists(carpetaJuego))
                    Process.Start("explorer.exe", carpetaJuego);
                else
                    MessageBox.Show("La carpeta del juego no existe:\n" + carpetaJuego);
            };

            // Botón Desinstalar
            Button btnDesinstalar = new Button
            {
                Size = new Size(200, 40),
                Location = new Point(620, 220),
                Text = "Desinstalar",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnDesinstalar.Click += (s, e) =>
            {
                string rutaUnins = Path.Combine(carpetaJuego, "unins000.exe");
                if (File.Exists(rutaUnins))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = rutaUnins,
                        WorkingDirectory = carpetaJuego,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("El desinstalador no existe:\n" + rutaUnins);
                }
            };

            panelJuego.Controls.Add(logo);
            panelJuego.Controls.Add(titulo);
            panelJuego.Controls.Add(desc);
            panelJuego.Controls.Add(btnAccion);
            panelJuego.Controls.Add(btnAbrirCarpeta);
            panelJuego.Controls.Add(btnDesinstalar);

            // Ajuste dinámico del TextBox
            panelJuego.Resize += (s, e) =>
            {
                desc.Width = panelJuego.Width - 200;
            };

            ActualizarBoton(rutaEjecutable);

            this.MinimumSize = new Size(800, 400);
        }

        private void CheckFileTimer_Tick(object sender, EventArgs e)
        {
            if (juegoSeleccionado != null)
            {
                string carpetaJuego = Path.GetFullPath(juegoSeleccionado.ubicacion.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
                string rutaEjecutable = Path.Combine(carpetaJuego, juegoSeleccionado.archivo_ejecutable);
                ActualizarBoton(rutaEjecutable);
            }
        }

        private void ActualizarBoton(string rutaEjecutable)
        {
            if (File.Exists(rutaEjecutable))
            {
                btnAccion.Text = "Abrir";
                btnAccion.Click -= Descargar_Click;
                btnAccion.Click -= Abrir_Click;
                btnAccion.Click += Abrir_Click;
            }
            else
            {
                btnAccion.Text = "Descargar";
                btnAccion.Click -= Abrir_Click;
                btnAccion.Click -= Descargar_Click;
                btnAccion.Click += Descargar_Click;
            }
        }

        private void Abrir_Click(object sender, EventArgs e)
        {
            string carpetaJuego = Path.GetFullPath(juegoSeleccionado.ubicacion.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
            string rutaEjecutable = Path.Combine(carpetaJuego, juegoSeleccionado.archivo_ejecutable);

            if (File.Exists(rutaEjecutable))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaEjecutable,
                    WorkingDirectory = carpetaJuego,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("El archivo no existe en la ubicación indicada:\n" + rutaEjecutable);
            }
        }

        private async void Descargar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Deseas descargar este juego?", "Descargar juego", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            string rutaDescarga = Path.Combine(downloadsDir, juegoSeleccionado.archivoDescargado);
            await DescargarJuego(juegoSeleccionado, rutaDescarga);

            string carpetaJuego = Path.GetFullPath(juegoSeleccionado.ubicacion.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
            Directory.CreateDirectory(carpetaJuego);

            Process.Start(new ProcessStartInfo
            {
                FileName = rutaDescarga,
                WorkingDirectory = carpetaJuego,
                UseShellExecute = true
            });
        }

        private async Task DescargarJuego(Juego juego, string rutaDestino)
        {
            using (HttpClient http = new HttpClient())
            {
                byte[] data = await http.GetByteArrayAsync(juego.descargar);
                string carpeta = Path.GetDirectoryName(rutaDestino);
                Directory.CreateDirectory(carpeta);
                File.WriteAllBytes(rutaDestino, data);
            }
            MessageBox.Show("Descarga completada.");
        }

        private void webOpenShare1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/acierto-incomodo/StormLibraryV2/releases/latest",
                UseShellExecute = true
            });
        }
    }
}
