
namespace OmegaPackageInstaller
{
    partial class OmegaSettingsInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OmegaSettingsInstaller));
            OmegaSettingsInstaller.textbox_console = new System.Windows.Forms.TextBox();
            this.button_complete = new System.Windows.Forms.Button();
            this.start_timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // textbox_console
            // 
            OmegaSettingsInstaller.textbox_console.BackColor = System.Drawing.Color.White;
            OmegaSettingsInstaller.textbox_console.Location = new System.Drawing.Point(12, 12);
            OmegaSettingsInstaller.textbox_console.Multiline = true;
            OmegaSettingsInstaller.textbox_console.Name = "textbox_console";
            OmegaSettingsInstaller.textbox_console.ReadOnly = true;
            OmegaSettingsInstaller.textbox_console.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            OmegaSettingsInstaller.textbox_console.Size = new System.Drawing.Size(799, 539);
            OmegaSettingsInstaller.textbox_console.TabIndex = 0;
            // 
            // button_complete
            // 
            this.button_complete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(129)))), ((int)(((byte)(162)))));
            this.button_complete.FlatAppearance.BorderColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button_complete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_complete.Location = new System.Drawing.Point(335, 606);
            this.button_complete.Name = "button_complete";
            this.button_complete.Size = new System.Drawing.Size(120, 23);
            this.button_complete.TabIndex = 5;
            this.button_complete.Text = "Finish";
            this.button_complete.UseVisualStyleBackColor = false;
            this.button_complete.Visible = false;
            this.button_complete.Click += new System.EventHandler(this.button_complete_Click);
            // 
            // start_timer
            // 
            this.start_timer.Tick += new System.EventHandler(this.start_timer_Tick);
            // 
            // OmegaSettingsInstaller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(827, 692);
            this.ControlBox = false;
            this.Controls.Add(this.button_complete);
            this.Controls.Add(OmegaSettingsInstaller.textbox_console);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OmegaSettingsInstaller";
            this.Text = "OmegaSettingsInstaller";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        static public System.Windows.Forms.TextBox textbox_console;
        private System.Windows.Forms.Button button_complete;
        private System.Windows.Forms.Timer start_timer;
    }
}

