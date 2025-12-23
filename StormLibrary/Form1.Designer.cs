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
            webOpenShare1 = new Button();
            SuspendLayout();
            // 
            // listGames
            // 
            resources.ApplyResources(listGames, "listGames");
            listGames.BackColor = SystemColors.InactiveCaptionText;
            listGames.ForeColor = SystemColors.Control;
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
            // webOpenShare1
            // 
            resources.ApplyResources(webOpenShare1, "webOpenShare1");
            webOpenShare1.BackColor = SystemColors.ActiveCaptionText;
            webOpenShare1.Name = "webOpenShare1";
            webOpenShare1.UseVisualStyleBackColor = false;
            webOpenShare1.Click += webOpenShare1_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            BackColor = SystemColors.ActiveCaptionText;
            Controls.Add(webOpenShare1);
            Controls.Add(listGames);
            Controls.Add(labelStatus);
            Controls.Add(panelJuego);
            ForeColor = SystemColors.Control;
            Name = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private Button webOpenShare1;
    }
}
