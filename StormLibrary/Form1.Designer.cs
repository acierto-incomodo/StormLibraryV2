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
            listGames = new ListBox();
            labelStatus = new Label();
            panelJuego = new Panel();
            SuspendLayout();
            // 
            // listGames
            // 
            listGames.Font = new Font("Segoe UI", 12F);
            listGames.ItemHeight = 28;
            listGames.Location = new Point(20, 20);
            listGames.Name = "listGames";
            listGames.Size = new Size(300, 396);
            listGames.TabIndex = 0;
            listGames.SelectedIndexChanged += listGames_SelectedIndexChanged;
            // 
            // labelStatus
            // 
            labelStatus.Font = new Font("Segoe UI", 10F);
            labelStatus.Location = new Point(20, 440);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(300, 30);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "Cargando...";
            // 
            // panelJuego
            // 
            panelJuego.AutoSize = true;
            panelJuego.Location = new Point(340, 20);
            panelJuego.Name = "panelJuego";
            panelJuego.Size = new Size(800, 400);
            panelJuego.TabIndex = 2;
            // 
            // Form1
            // 
            AutoSize = true;
            ClientSize = new Size(1161, 500);
            Controls.Add(listGames);
            Controls.Add(labelStatus);
            Controls.Add(panelJuego);
            MinimumSize = new Size(878, 547);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "StormLibrary Launcher";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
