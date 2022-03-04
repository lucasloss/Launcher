namespace Launcher
{
    partial class FormEditModuleInfo
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
            this.labelDisplayName = new System.Windows.Forms.Label();
            this.textBoxDisplayName = new System.Windows.Forms.TextBox();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.labelPath = new System.Windows.Forms.Label();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.checkBoxIsService = new System.Windows.Forms.CheckBox();
            this.checkBoxRunOnStartAll = new System.Windows.Forms.CheckBox();
            this.textBoxArguments = new System.Windows.Forms.TextBox();
            this.labelArguments = new System.Windows.Forms.Label();
            this.checkBoxAllowMultipleInstances = new System.Windows.Forms.CheckBox();
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
            this.flowLayoutPanelBottom.Location = new System.Drawing.Point(0, 230);
            this.flowLayoutPanelBottom.Name = "flowLayoutPanelBottom";
            this.flowLayoutPanelBottom.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanelBottom.Size = new System.Drawing.Size(441, 29);
            this.flowLayoutPanelBottom.TabIndex = 20;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(357, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(276, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // labelDisplayName
            // 
            this.labelDisplayName.AutoSize = true;
            this.labelDisplayName.Location = new System.Drawing.Point(12, 14);
            this.labelDisplayName.Name = "labelDisplayName";
            this.labelDisplayName.Size = new System.Drawing.Size(80, 15);
            this.labelDisplayName.TabIndex = 11;
            this.labelDisplayName.Text = "Display Name";
            // 
            // textBoxDisplayName
            // 
            this.textBoxDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDisplayName.Location = new System.Drawing.Point(12, 32);
            this.textBoxDisplayName.Name = "textBoxDisplayName";
            this.textBoxDisplayName.Size = new System.Drawing.Size(417, 23);
            this.textBoxDisplayName.TabIndex = 12;
            // 
            // textBoxPath
            // 
            this.textBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPath.Location = new System.Drawing.Point(12, 76);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(369, 23);
            this.textBoxPath.TabIndex = 14;
            // 
            // labelPath
            // 
            this.labelPath.AutoSize = true;
            this.labelPath.Location = new System.Drawing.Point(12, 58);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(31, 15);
            this.labelPath.TabIndex = 13;
            this.labelPath.Text = "Path";
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectFile.Location = new System.Drawing.Point(387, 76);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(42, 23);
            this.buttonSelectFile.TabIndex = 15;
            this.buttonSelectFile.Text = "...";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsService
            // 
            this.checkBoxIsService.AutoSize = true;
            this.checkBoxIsService.Location = new System.Drawing.Point(12, 149);
            this.checkBoxIsService.Name = "checkBoxIsService";
            this.checkBoxIsService.Size = new System.Drawing.Size(74, 19);
            this.checkBoxIsService.TabIndex = 17;
            this.checkBoxIsService.Text = "Is Service";
            this.checkBoxIsService.UseVisualStyleBackColor = true;
            // 
            // checkBoxRunOnStartAll
            // 
            this.checkBoxRunOnStartAll.AutoSize = true;
            this.checkBoxRunOnStartAll.Location = new System.Drawing.Point(12, 199);
            this.checkBoxRunOnStartAll.Name = "checkBoxRunOnStartAll";
            this.checkBoxRunOnStartAll.Size = new System.Drawing.Size(143, 19);
            this.checkBoxRunOnStartAll.TabIndex = 18;
            this.checkBoxRunOnStartAll.Text = "Run on \'Run Checked\'";
            this.checkBoxRunOnStartAll.UseVisualStyleBackColor = true;
            // 
            // textBoxArguments
            // 
            this.textBoxArguments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArguments.Location = new System.Drawing.Point(12, 120);
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.Size = new System.Drawing.Size(417, 23);
            this.textBoxArguments.TabIndex = 16;
            // 
            // labelArguments
            // 
            this.labelArguments.AutoSize = true;
            this.labelArguments.Location = new System.Drawing.Point(12, 102);
            this.labelArguments.Name = "labelArguments";
            this.labelArguments.Size = new System.Drawing.Size(66, 15);
            this.labelArguments.TabIndex = 21;
            this.labelArguments.Text = "Arguments";
            // 
            // checkBoxAllowMultipleInstances
            // 
            this.checkBoxAllowMultipleInstances.AutoSize = true;
            this.checkBoxAllowMultipleInstances.Location = new System.Drawing.Point(12, 174);
            this.checkBoxAllowMultipleInstances.Name = "checkBoxAllowMultipleInstances";
            this.checkBoxAllowMultipleInstances.Size = new System.Drawing.Size(155, 19);
            this.checkBoxAllowMultipleInstances.TabIndex = 22;
            this.checkBoxAllowMultipleInstances.Text = "Allow Multiple Instances";
            this.checkBoxAllowMultipleInstances.UseVisualStyleBackColor = true;
            // 
            // FormEditModuleInfo
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(441, 259);
            this.Controls.Add(this.checkBoxAllowMultipleInstances);
            this.Controls.Add(this.textBoxArguments);
            this.Controls.Add(this.labelArguments);
            this.Controls.Add(this.checkBoxRunOnStartAll);
            this.Controls.Add(this.checkBoxIsService);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.textBoxDisplayName);
            this.Controls.Add(this.labelDisplayName);
            this.Controls.Add(this.flowLayoutPanelBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditModuleInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Module Information";
            this.flowLayoutPanelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FlowLayoutPanel flowLayoutPanelBottom;
        private Button buttonCancel;
        private Button buttonSave;
        private Label labelDisplayName;
        private TextBox textBoxDisplayName;
        private TextBox textBoxPath;
        private Label labelPath;
        private Button buttonSelectFile;
        private CheckBox checkBoxIsService;
        private CheckBox checkBoxRunOnStartAll;
        private TextBox textBoxArguments;
        private Label labelArguments;
        private CheckBox checkBoxAllowMultipleInstances;
    }
}