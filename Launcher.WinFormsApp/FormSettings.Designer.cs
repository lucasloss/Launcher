namespace Launcher
{
    partial class FormSettings
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
            this.flowLayoutPanelBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxDefaultStartupProject = new System.Windows.Forms.TextBox();
            this.labelDefaultProject = new System.Windows.Forms.Label();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.flowLayoutPanelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelBottom
            // 
            this.flowLayoutPanelBottom.AutoSize = true;
            this.flowLayoutPanelBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelBottom.Controls.Add(this.buttonCancel);
            this.flowLayoutPanelBottom.Controls.Add(this.buttonSave);
            this.flowLayoutPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanelBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanelBottom.Location = new System.Drawing.Point(0, 65);
            this.flowLayoutPanelBottom.Name = "flowLayoutPanelBottom";
            this.flowLayoutPanelBottom.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanelBottom.Size = new System.Drawing.Size(441, 31);
            this.flowLayoutPanelBottom.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.AutoSize = true;
            this.buttonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonCancel.Location = new System.Drawing.Point(379, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(53, 25);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSave.Location = new System.Drawing.Point(332, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(41, 25);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // textBoxDefaultStartupProject
            // 
            this.textBoxDefaultStartupProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDefaultStartupProject.Location = new System.Drawing.Point(12, 27);
            this.textBoxDefaultStartupProject.Name = "textBoxDefaultStartupProject";
            this.textBoxDefaultStartupProject.Size = new System.Drawing.Size(369, 23);
            this.textBoxDefaultStartupProject.TabIndex = 0;
            // 
            // labelDefaultProject
            // 
            this.labelDefaultProject.AutoSize = true;
            this.labelDefaultProject.Location = new System.Drawing.Point(12, 9);
            this.labelDefaultProject.Name = "labelDefaultProject";
            this.labelDefaultProject.Size = new System.Drawing.Size(203, 15);
            this.labelDefaultProject.TabIndex = 21;
            this.labelDefaultProject.Text = "Load the following project on startup";
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFile.Location = new System.Drawing.Point(387, 27);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(42, 23);
            this.buttonSelectFile.TabIndex = 1;
            this.buttonSelectFile.Text = "...";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(441, 96);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.textBoxDefaultStartupProject);
            this.Controls.Add(this.labelDefaultProject);
            this.Controls.Add(this.flowLayoutPanelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.flowLayoutPanelBottom.ResumeLayout(false);
            this.flowLayoutPanelBottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanelBottom;
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDefaultStartupProject;
        private Label labelDefaultProject;
        private Button buttonSelectFile;
    }
}