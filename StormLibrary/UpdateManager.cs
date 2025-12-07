using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StormLibrary
{
    public class UpdateManager
    {
        public string LogosDir { get; private set; }

        public UpdateManager()
        {
            LogosDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StormGamesStudios",
                "StormLibraryV2",
                "logos"
            );
            Directory.CreateDirectory(LogosDir);
        }

        // Comprueba versión y descarga solo si hay cambios
        public async Task CheckAndDownloadFiles(string dataDir)
        {
            Directory.CreateDirectory(dataDir);

            string urlGames = "https://github.com/acierto-incomodo/StormLibraryV2/releases/latest/download/games.json";
            string urlVersion = "https://github.com/acierto-incomodo/StormLibraryV2/releases/latest/download/gamesVersion.txt";

            string rutaGames = Path.Combine(dataDir, "games.json");
            string rutaVersion = Path.Combine(dataDir, "gamesVersion.txt");

            string versionLocal = File.Exists(rutaVersion) ? await File.ReadAllTextAsync(rutaVersion) : "";
            string versionRemota = await new HttpClient().GetStringAsync(urlVersion);

            if (versionLocal.Trim() != versionRemota.Trim())
            {
                // Descargar y sobrescribir
                await DescargarArchivo(urlGames, rutaGames);
                await DescargarArchivo(urlVersion, rutaVersion);

                // Descargar logos de cada juego
                var juegos = await LoadGames(dataDir);
                foreach (var juego in juegos)
                {
                    if (!string.IsNullOrEmpty(juego.logo))
                    {
                        string logoDestino = Path.Combine(LogosDir, juego.nombre.Replace(" ", "") + ".png");
                        await DescargarArchivo(juego.logo, logoDestino);
                    }
                }
            }
        }

        private async Task DescargarArchivo(string url, string destino)
        {
            using (HttpClient http = new HttpClient())
            {
                byte[] data = await http.GetByteArrayAsync(url);
                string carpeta = Path.GetDirectoryName(destino);
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
                File.WriteAllBytes(destino, data);
            }
        }

        public async Task<List<Juego>> LoadGames(string dataDir)
        {
            string rutaGames = Path.Combine(dataDir, "games.json");
            if (!File.Exists(rutaGames)) return new List<Juego>();

            string json = await File.ReadAllTextAsync(rutaGames);
            return JsonSerializer.Deserialize<List<Juego>>(json) ?? new List<Juego>();
        }
    }
}
