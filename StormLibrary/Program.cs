using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace StormLibrary
{
    internal static class Program
    {
        // Aquí se guardará la URL tipo:
        // stormlibraryv2://game/the-shooter
        public static string DeepLinkUrl { get; private set; }

        /// <summary>
        /// Punto de entrada principal de la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Si la app se abre desde el navegador, llegará aquí
            if (args != null && args.Length > 0)
            {
                DeepLinkUrl = args[0];
            }

            // Eliminar archivos de la carpeta `downloads` al iniciar
            try
            {
                string downloadsDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "StormGamesStudios",
                    "StormLibraryV2",
                    "downloads"
                );

                if (Directory.Exists(downloadsDir))
                {
                    // Eliminar archivos directos
                    foreach (var file in Directory.GetFiles(downloadsDir, "*", SearchOption.TopDirectoryOnly))
                    {
                        try { File.Delete(file); } catch { /* ignorar errores individuales */ }
                    }

                    // Eliminar subcarpetas y su contenido (si las hay)
                    foreach (var dir in Directory.GetDirectories(downloadsDir))
                    {
                        try { Directory.Delete(dir, true); } catch { /* ignorar errores individuales */ }
                    }
                }
            }
            catch
            {
                // No hacer nada: si falla el borrado no impedimos que la app arranque
            }

            // Asegurar que solo haya una instancia de la aplicación
            bool createdNew;
            using (var mutex = new Mutex(true, "StormLibraryV2_SingleInstance_Mutex", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show(
                        "La aplicación ya se está ejecutando.",
                        "Instancia existente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }
    }
}
