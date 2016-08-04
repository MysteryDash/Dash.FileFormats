namespace MysteryDash.Cl3Editor
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.Save = new System.Windows.Forms.Button();
            this.InstructionsLabel = new System.Windows.Forms.Label();
            this.Cl3Files = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(12, 238);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(335, 23);
            this.Save.TabIndex = 5;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // InstructionsLabel
            // 
            this.InstructionsLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionsLabel.Location = new System.Drawing.Point(12, 8);
            this.InstructionsLabel.Name = "InstructionsLabel";
            this.InstructionsLabel.Size = new System.Drawing.Size(335, 121);
            this.InstructionsLabel.TabIndex = 4;
            this.InstructionsLabel.Text = resources.GetString("InstructionsLabel.Text");
            // 
            // Cl3Files
            // 
            this.Cl3Files.AllowDrop = true;
            this.Cl3Files.FullRowSelect = true;
            this.Cl3Files.Location = new System.Drawing.Point(12, 137);
            this.Cl3Files.Name = "Cl3Files";
            this.Cl3Files.Size = new System.Drawing.Size(335, 95);
            this.Cl3Files.TabIndex = 3;
            this.Cl3Files.UseCompatibleStateImageBehavior = false;
            this.Cl3Files.View = System.Windows.Forms.View.List;
            this.Cl3Files.DragDrop += new System.Windows.Forms.DragEventHandler(this.Cl3Files_DragDrop);
            this.Cl3Files.DragEnter += new System.Windows.Forms.DragEventHandler(this.Cl3Files_DragEnter);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 268);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.InstructionsLabel);
            this.Controls.Add(this.Cl3Files);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "Cl3 Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Label InstructionsLabel;
        private System.Windows.Forms.ListView Cl3Files;
    }
}

