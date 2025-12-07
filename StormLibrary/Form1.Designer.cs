namespace StormLibrary
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listGames;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Panel panelJuego; // panel donde se cargará la info del juego

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            listGames = new ListBox();
            labelStatus = new Label();
            panelJuego = new Panel();
            SuspendLayout();
            // 
            // listGames
            // 
            resources.ApplyResources(listGames, "listGames");
            listGames.Name = "listGames";
            listGames.SelectedIndexChanged += listGames_SelectedIndexChanged;
            // 
            // labelStatus
            // 
            resources.ApplyResources(labelStatus, "labelStatus");
            labelStatus.Name = "labelStatus";
            // 
            // panelJuego
            // 
            resources.ApplyResources(panelJuego, "panelJuego");
            panelJuego.Name = "panelJuego";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            Controls.Add(listGames);
            Controls.Add(labelStatus);
            Controls.Add(panelJuego);
            Name = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
