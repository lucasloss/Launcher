namespace Launcher
{
    /// <summary>
    /// Represents a window that allows the editing of a <see cref="UserPreferences"/> instance.
    /// </summary>
    public partial class FormSettings : Form
    {
        private readonly UserPreferences _userPreferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormSettings"/> class.
        /// </summary>
        /// <param name="userPreferences">The <see cref="UserPreferences"/> instance to be modified.</param>
        public FormSettings(UserPreferences userPreferences)
        {
            _userPreferences = userPreferences;

            InitializeComponent();

            ConfigureForm();
            MapToView();
        }

        private void MapToView()
        {
            if (!string.IsNullOrWhiteSpace(_userPreferences.DefaultStartupProjectPath))
            {
                textBoxDefaultStartupProject.Text = _userPreferences.DefaultStartupProjectPath;
            }
        }

        private void ConfigureForm()
        {
            buttonSave.Click += ButtonSave_Click;
            buttonCancel.Click += ButtonCancel_Click;
            buttonSelectFile.Click += ButtonSelectFile_Click;
        }

        private void ButtonSelectFile_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new()
            {
                Filter = "JSON Files | *.json",
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDefaultStartupProject.Text = openFileDialog.FileName;
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
            if (!string.IsNullOrWhiteSpace(textBoxDefaultStartupProject.Text))
            {
                _userPreferences.DefaultStartupProjectPath = textBoxDefaultStartupProject.Text;
            }
            else
            {
                _userPreferences.DefaultStartupProjectPath = null;
            }

            return true;
        }
    }
}
