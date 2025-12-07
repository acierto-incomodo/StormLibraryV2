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
        private readonly HttpClient http = new HttpClient();

        public string AppDir => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StormGamesStudios",
            "NewGameDir"
        );

        public string LocalJsonPath => Path.Combine(AppDir, "games.json");
        public string LocalVersionPath => Path.Combine(AppDir, "gamesVersion.txt");
        public string LogosDir => Path.Combine(AppDir, "logos");

        public string RemoteVersionUrl = "URL_A_TU_gamesVersion.txt";
        public string RemoteGamesJsonUrl = "URL_A_TU_games.json";

        public async Task<List<Juego>> CheckUpdates()
        {
            Directory.CreateDirectory(AppDir);
            Directory.CreateDirectory(LogosDir);

            string remoteVersion = "";
            bool mustUpdate = true;

            try
            {
                remoteVersion = await http.GetStringAsync(RemoteVersionUrl);

                if (File.Exists(LocalVersionPath))
                {
                    string localVer = File.ReadAllText(LocalVersionPath);
                    if (remoteVersion.Trim() == localVer.Trim())
                        mustUpdate = false;
                }

                if (mustUpdate)
                {
                    File.WriteAllText(LocalVersionPath, remoteVersion);

                    string json = await http.GetStringAsync(RemoteGamesJsonUrl);
                    File.WriteAllText(LocalJsonPath, json);

                    var games = JsonSerializer.Deserialize<List<Juego>>(json);

                    await DownloadLogos(games);

                    return games;
                }
            }
            catch
            {
                // Offline
            }

            if (File.Exists(LocalJsonPath))
            {
                string json = File.ReadAllText(LocalJsonPath);
                return JsonSerializer.Deserialize<List<Juego>>(json);
            }

            return new List<Juego>();
        }

        private async Task DownloadLogos(List<Juego> juegos)
        {
            foreach (var game in juegos)
            {
                try
                {
                    byte[] data = await http.GetByteArrayAsync(game.logo);
                    string name = game.nombre.Replace(" ", "") + ".png";
                    File.WriteAllBytes(Path.Combine(LogosDir, name), data);
                }
                catch { }
            }
        }
    }
}
