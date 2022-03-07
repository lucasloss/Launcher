using Launcher.Core;

namespace Launcher
{
    /// <summary>
    /// Represents a window that allows the edit of a <see cref="Launcher.Core.ModuleInfo"/> instance.
    /// </summary>
    public partial class FormEditModuleInfo : Form
    {
        private readonly ModuleInfo _originalModuleInfo = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormEditModuleInfo"/> class.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be edited.
        /// If the <paramref name="moduleInfo"/> is null, a new instance of <see cref="ModuleInfo"/> will be created.</param>
        public FormEditModuleInfo(ModuleInfo? moduleInfo)
        {
            if (moduleInfo is null)
            {
                ModuleInfo = new ModuleInfo();
            }
            else
            {
                ModuleInfo = moduleInfo;
                SaveOriginal(ModuleInfo);
            }

            InitializeComponent();

            ConfigureForm();
            MapToView();
        }

        /// <summary>
        /// Gets the <see cref="ModuleInfo"/> associated with the form.
        /// </summary>
        public ModuleInfo ModuleInfo { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ModuleInfo"/> has changes.
        /// </summary>
        public bool ModuleInfoChanged { get; private set; }

        private void SaveOriginal(ModuleInfo moduleInfo)
        {
            _originalModuleInfo.RunOnStartAll = moduleInfo.RunOnStartAll;
            _originalModuleInfo.IsService = moduleInfo.IsService;
            _originalModuleInfo.DisplayName = moduleInfo.DisplayName;
            _originalModuleInfo.Path = moduleInfo.Path;
            _originalModuleInfo.Arguments = moduleInfo.Arguments;
        }

        private bool ModuleInfoHasChanged()
        {
            return _originalModuleInfo.RunOnStartAll != ModuleInfo.RunOnStartAll
                || _originalModuleInfo.IsService != ModuleInfo.IsService
                || _originalModuleInfo.DisplayName != ModuleInfo.DisplayName
                || _originalModuleInfo.Path != ModuleInfo.Path
                || _originalModuleInfo.Arguments != ModuleInfo.Arguments;
        }

        private void MapToView()
        {
            textBoxDisplayName.Text = ModuleInfo.DisplayName;
            textBoxPath.Text = ModuleInfo.Path;
            textBoxArguments.Text = ModuleInfo.Arguments;
            checkBoxIsService.Checked = ModuleInfo.IsService;
            checkBoxAllowMultipleInstances.Checked = ModuleInfo.AllowMultipleInstances;
            checkBoxRunOnStartAll.Checked = ModuleInfo.RunOnStartAll;
        }

        private void ConfigureForm()
        {
            buttonSave.Image = Resources.Save_16x;
            buttonCancel.Image = Resources.Cancel_16x;

            buttonSave.Click += ButtonSave_Click;
            buttonCancel.Click += ButtonCancel_Click;
            buttonSelectFile.Click += ButtonSelectFile_Click;
            checkBoxIsService.CheckedChanged += CheckBoxIsService_CheckedChanged;
        }

        private void CheckBoxIsService_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkBoxIsService.Checked)
            {
                checkBoxAllowMultipleInstances.Checked = false;
                checkBoxAllowMultipleInstances.Enabled = false;
            }
            else
            {
                checkBoxAllowMultipleInstances.Enabled = true;
            }
        }

        private void ButtonSelectFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = dialog.FileName;
            }
        }

        private void ButtonCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            if (MapToObject())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private bool MapToObject()
        {
            if (string.IsNullOrWhiteSpace(textBoxDisplayName.Text))
            {
                MessageBox.Show(
                    $"{labelDisplayName.Text} cannot be null or empty.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxPath.Text))
            {
                MessageBox.Show(
                    $"{labelPath.Text} cannot be null or empty.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }

            ModuleInfo.RunOnStartAll = checkBoxRunOnStartAll.Checked;
            ModuleInfo.IsService = checkBoxIsService.Checked;
            ModuleInfo.AllowMultipleInstances = checkBoxAllowMultipleInstances.Checked;
            ModuleInfo.DisplayName = textBoxDisplayName.Text;
            ModuleInfo.Path = textBoxPath.Text;
            ModuleInfo.Arguments = null;

            if (!string.IsNullOrWhiteSpace(textBoxArguments.Text))
            {
                ModuleInfo.Arguments = textBoxArguments.Text;
            }

            ModuleInfoChanged = ModuleInfoHasChanged();

            return true;
        }
    }
}
