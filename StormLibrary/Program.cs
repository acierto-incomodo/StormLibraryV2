using System;
using System.Windows.Forms;

namespace StormLibrary
{
    internal static class Program
    {
        // Aquí se guardará la URL tipo:
        // stormlibraryv2://game/the-shooter
        public static string DeepLinkUrl { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Si la app se abre desde el navegador, llegará aquí
            if (args != null && args.Length > 0)
            {
                DeepLinkUrl = args[0];
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
