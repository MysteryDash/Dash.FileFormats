namespace MysteryDash.PacEditor
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
            this.PacFiles = new System.Windows.Forms.ListView();
            this.InstructionsLabel = new System.Windows.Forms.Label();
            this.Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PacFiles
            // 
            this.PacFiles.AllowDrop = true;
            this.PacFiles.FullRowSelect = true;
            this.PacFiles.Location = new System.Drawing.Point(12, 123);
            this.PacFiles.Name = "PacFiles";
            this.PacFiles.Size = new System.Drawing.Size(335, 436);
            this.PacFiles.TabIndex = 0;
            this.PacFiles.UseCompatibleStateImageBehavior = false;
            this.PacFiles.View = System.Windows.Forms.View.List;
            this.PacFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.PacFiles_DragDrop);
            this.PacFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.PacFiles_DragEnter);
            // 
            // InstructionsLabel
            // 
            this.InstructionsLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionsLabel.Location = new System.Drawing.Point(12, 9);
            this.InstructionsLabel.Name = "InstructionsLabel";
            this.InstructionsLabel.Size = new System.Drawing.Size(335, 111);
            this.InstructionsLabel.TabIndex = 1;
            this.InstructionsLabel.Text = resources.GetString("InstructionsLabel.Text");
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(12, 565);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(335, 23);
            this.Save.TabIndex = 2;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 594);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.InstructionsLabel);
            this.Controls.Add(this.PacFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "Pac Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView PacFiles;
        private System.Windows.Forms.Label InstructionsLabel;
        private System.Windows.Forms.Button Save;
    }
}

