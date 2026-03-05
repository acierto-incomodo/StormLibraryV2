using Microsoft.Win32;
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

            // Suscribir el evento para el botón usarStormStore
            usarStormStore.Click += usarStormStore_Click;

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

            ActualizarLabelVersion();
            RegistrarProtocolo();
            ProcesarDeepLink();
            // Mostrar alerta de instalación de StormStore cada vez que se inicia la aplicación
            await PromptToInstallStormStoreIfMissingAsync();
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
            Label titulo = new Label
            {
                Text = juego.nombre,
                Font = new Font("Segoe UI", 18),
                Location = new Point(180, 20),
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

            Label lblRequisitos = new Label
            {
                Text = "Requisitos:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(180, desc.Bottom + 10),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
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
                Size = new Size(180, 40),
                Location = new Point(180, 320),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Botón Abrir carpeta
            Button btnAbrirCarpeta = new Button
            {
                Size = new Size(180, 40),
                Location = new Point(390, 320),
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
                Size = new Size(180, 40),
                Location = new Point(600, 320),
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

            // Botón Compartir Juego
            Button btnCompartir = new Button
            {
                Size = new Size(180, 40),
                Location = new Point(180, 370), // Ajusta según tu layout
                Text = "Compartir Juego",
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            btnCompartir.Click += (s, e) =>
            {
                // URL web del juego
                string urlJuego = $"https://stormgamesstudios.vercel.app/juegos/juegos-url/{juego.game_id}/play";

                // Texto a copiar
                string textoCopiado = $"Juega conmigo a {urlJuego}";

                // Copiar al portapapeles
                Clipboard.SetText(textoCopiado);

                // Feedback visual
                string originalText = btnCompartir.Text;
                btnCompartir.Text = "Enlace del juego copiado";

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 5000;
                timer.Tick += (senderTimer, args) =>
                {
                    btnCompartir.Text = originalText;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            };

            panelJuego.Controls.Add(btnCompartir);
            panelJuego.Controls.Add(logo);
            panelJuego.Controls.Add(titulo);
            panelJuego.Controls.Add(desc);
            panelJuego.Controls.Add(lblRequisitos);
            panelJuego.Controls.Add(txtRequisitos);
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
            string carpetaJuego = Path.GetFullPath(
                juegoSeleccionado.ubicacion.Replace(
                    "%appdata%",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                )
            );

            string rutaEjecutable = Path.Combine(carpetaJuego, juegoSeleccionado.archivo_ejecutable);

            if (File.Exists(rutaEjecutable))
            {
                AbrirSteamSiEsNecesario(juegoSeleccionado);

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
            var result = MessageBox.Show(
                "¿Deseas descargar este juego?",
                "Descargar juego",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            labelStatus.Text = "Descargando instalador...";

            // Cambiar cursor a "espera"
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string rutaDescarga = Path.Combine(downloadsDir, juegoSeleccionado.archivoDescargado);
                await DescargarJuego(juegoSeleccionado, rutaDescarga);

                string carpetaJuego = Path.GetFullPath(
                    juegoSeleccionado.ubicacion.Replace(
                        "%appdata%",
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    )
                );

                Directory.CreateDirectory(carpetaJuego);

                if (File.Exists(rutaDescarga))
                {
                    // Abrir el instalador
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = rutaDescarga,
                        WorkingDirectory = carpetaJuego,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show(
                        "No se encontró el archivo descargado. Asegúrate de que la descarga se completó correctamente.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar o abrir el juego:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Volver cursor a normal
                Cursor.Current = Cursors.Default;
            }
        }

        private async Task DescargarJuego(Juego juego, string rutaDestino)
        {
            try
            {
                // Cambiar cursor a "espera"
                Cursor.Current = Cursors.WaitCursor;

                using (HttpClient http = new HttpClient())
                {
                    byte[] data = await http.GetByteArrayAsync(juego.descargar);
                    string carpeta = Path.GetDirectoryName(rutaDestino);
                    Directory.CreateDirectory(carpeta);
                    File.WriteAllBytes(rutaDestino, data);
                }

                MessageBox.Show("Descarga completada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar el juego:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Volver cursor a normal
                Cursor.Current = Cursors.Default;
            }

            ActualizarLabelVersion();
            AbrirSteamSiEsNecesario(juego);
        }

        private void webOpenShare1_Click(object sender, EventArgs e)
        {
            string enlace = "https://github.com/acierto-incomodo/StormLibraryV2/releases/latest";

            // Copiar al portapapeles
            Clipboard.SetText(enlace);

            // Cambiar texto del botón temporalmente
            Button btn = sender as Button;
            if (btn == null) return;

            string originalText = btn.Text;
            btn.Text = "Enlace copiado";

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000; // 5 segundos
            timer.Tick += (s, ev) =>
            {
                btn.Text = originalText;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void AbrirSteamSiEsNecesario(Juego juego)
        {
            if (juego.steam?.ToLower() != "si")
                return;

            string steamPath = @"C:\Program Files (x86)\Steam\Steam.exe";

            if (File.Exists(steamPath))
            {
                // Evita abrir Steam dos veces
                if (Process.GetProcessesByName("steam").Length == 0)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = steamPath,
                        UseShellExecute = true
                    });
                }
            }
            else
            {
                MessageBox.Show(
                    "Este juego requiere Steam, pero no se encontró Steam instalado.",
                    "Steam no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void ActualizarLabelVersion()
        {
            string rutaVersion = Path.Combine(dataDir, "gamesVersion.txt");

            if (File.Exists(rutaVersion))
            {
                string version = File.ReadAllText(rutaVersion).Trim();
                labelStatus.Text = version;
            }
            else
            {
                labelStatus.Text = "Versión desconocida";
            }
        }

        private void RegistrarProtocolo()
        {
            string protocolo = "stormlibraryv2";
            string exePath = Application.ExecutablePath;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                $@"Software\Classes\{protocolo}"))
            {
                key.SetValue("", "URL:StormLibraryV2 Protocol");
                key.SetValue("URL Protocol", "");

                using (RegistryKey defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("", $"\"{exePath}\",1");
                }

                using (RegistryKey command = key.CreateSubKey(@"shell\open\command"))
                {
                    command.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }
        }

        // Mostrar alerta en el inicio si StormStore no está instalado y ofrecer instalarlo
        private async Task PromptToInstallStormStoreIfMissingAsync()
        {
            // Rutas a comprobar
            string appDataStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StormGamesStudios",
                "StormStore",
                "StormStore.exe"
            );

            string localAppDataStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "StormStore",
                "StormStore.exe"
            );

            string programFilesStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "StormStore",
                "StormStore.exe"
            );

            if (File.Exists(appDataStormStore) || File.Exists(localAppDataStormStore) || File.Exists(programFilesStormStore))
                return;

            var instalar = MessageBox.Show(
                "StormStore no está instalado. ¿Deseas instalar StormStore?",
                "Instalar StormStore",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (instalar != DialogResult.Yes)
                return;

            await DownloadAndRunStormStoreInstallerAsync();
        }

        private async Task DownloadAndRunStormStoreInstallerAsync()
        {
            string installerUrl = "https://github.com/acierto-incomodo/StormStore/releases/latest/download/StormStore-Setup.exe";
            string installerName = Path.GetFileName(new Uri(installerUrl).LocalPath);
            string destino = Path.Combine(downloadsDir, installerName);

            try
            {
                Directory.CreateDirectory(downloadsDir);

                if (!File.Exists(destino))
                {
                    labelStatus.Text = "Descargando StormStore...";
                    Cursor.Current = Cursors.WaitCursor;

                    using (HttpClient http = new HttpClient())
                    {
                        byte[] data = await http.GetByteArrayAsync(installerUrl);
                        File.WriteAllBytes(destino, data);
                    }

                    labelStatus.Text = "Descarga completada.";
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = destino,
                    UseShellExecute = true
                });

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar o ejecutar el instalador:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ProcesarDeepLink()
        {
            if (string.IsNullOrEmpty(Program.DeepLinkUrl))
                return;

            if (juegos == null || juegos.Count == 0)
                return;

            Uri uri;
            try
            {
                uri = new Uri(Program.DeepLinkUrl);
            }
            catch
            {
                return;
            }

            // Solo manejamos deep links tipo: stormlibraryv2://game/game-id
            if (!uri.Host.Equals("game", StringComparison.OrdinalIgnoreCase))
                return;

            string gameId = uri.AbsolutePath.Trim('/');
            if (string.IsNullOrEmpty(gameId))
                return;

            Juego juego = juegos.Find(j =>
                !string.IsNullOrEmpty(j.game_id) &&
                j.game_id.Equals(gameId, StringComparison.OrdinalIgnoreCase)
            );

            if (juego == null)
                return;

            // Seleccionamos el juego en la lista
            listGames.SelectedItem = juego;
            listGames.Focus();

            // Obtenemos la ruta del ejecutable
            string carpetaJuego = Path.GetFullPath(
                juego.ubicacion.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            );
            string rutaEjecutable = Path.Combine(carpetaJuego, juego.archivo_ejecutable);

            // Si el juego existe, lo abrimos
            if (File.Exists(rutaEjecutable))
            {
                AbrirSteamSiEsNecesario(juego);
                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaEjecutable,
                    WorkingDirectory = carpetaJuego,
                    UseShellExecute = true
                });
            }
            else
            {
                // Si no existe, mostramos mensaje
                MessageBox.Show(
                    $"El juego \"{juego.nombre}\" no está instalado.\nPor favor instálalo antes de abrirlo.",
                    "Juego no instalado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private async void reloadGameVersion_Click(object sender, EventArgs e)
        {
            try
            {
                // Cursor de espera
                Cursor.Current = Cursors.WaitCursor;
                labelStatus.Text = "Comprobando actualizaciones...";

                await updateManager.CheckAndDownloadFiles(dataDir);

                juegos = await updateManager.LoadGames(dataDir);

                listGames.DataSource = null;
                listGames.DataSource = juegos;
                listGames.DisplayMember = "nombre";

                ActualizarLabelVersion();

                MessageBox.Show(
                    "La lista de juegos está actualizada.",
                    "Actualización completada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al comprobar actualizaciones:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async void usarStormStore_Click(object sender, EventArgs e)
        {
            // Rutas a comprobar
            string appDataStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StormGamesStudios",
                "StormStore",
                "StormStore.exe"
            );

            string localAppDataStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "StormStore",
                "StormStore.exe"
            );

            string programFilesStormStore = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "StormStore",
                "StormStore.exe"
            );

            // Si ya existe en cualquiera de las ubicaciones, lo abrimos
            string existente = null;
            if (File.Exists(appDataStormStore))
                existente = appDataStormStore;
            else if (File.Exists(localAppDataStormStore))
                existente = localAppDataStormStore;
            else if (File.Exists(programFilesStormStore))
                existente = programFilesStormStore;

            if (existente != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = existente,
                        WorkingDirectory = Path.GetDirectoryName(existente),
                        UseShellExecute = true
                    });
                    // Cerrar esta aplicación tras lanzar StormStore
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir StormStore:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Si no está instalado: preguntar e iniciar instalador si el usuario acepta
            try
            {
                await PromptToInstallStormStoreIfMissingAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar o ejecutar el instalador:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
