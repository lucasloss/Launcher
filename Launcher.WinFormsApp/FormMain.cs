using Launcher.Core;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Launcher
{
    /// <summary>
    /// The main form of the application.
    /// </summary>
    public partial class FormMain : Form
    {
        private const int SW_SHOWNORMAL = 1;
        private readonly Bitmap _imageStatusStop16x = Resources.StatusStop_16x;
        private readonly Bitmap _imageStatusRun16x = Resources.StatusRun_16x;
        private readonly Bitmap _imageApplication16x = Resources.Application_16x;
        private readonly Bitmap _imageWindowsService16x = Resources.WindowsService_16x;
        private readonly Bitmap _imageBatchFile16x = Resources.BatchFile_16x;
        private readonly string _userPreferencesPath = Path.Combine(Application.StartupPath, "UserPreferences.json");
        private UserPreferences _userPreferences = new();
        private LauncherController _launcherController = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            LoadUserPreferences();
            CreateNewProject();

            Load += FormMain_Load;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr windowHandle);

        private void SaveUserPreferences()
        {
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
            };

            string contents = JsonConvert.SerializeObject(_userPreferences, jsonSerializerSettings);
            File.WriteAllText(_userPreferencesPath, contents, Encoding.UTF8);
        }

        private void FormMain_Load(object? sender, EventArgs e)
        {
            ConfigureForm();

            if (!string.IsNullOrWhiteSpace(_userPreferences.DefaultStartupProjectPath))
            {
                LoadProject(_userPreferences.DefaultStartupProjectPath);
            }
        }

        private void CreateNewProject()
        {
            _launcherController = new LauncherController();
            _launcherController.ModuleInfoChanged += LauncherController_ModuleInfoChanged;
            _launcherController.LauncherConfigurationStatusChanged += LauncherController_LauncherConfigurationStatusChanged;
            _launcherController.MonitoringChanged += LauncherController_MonitoringChanged;
            dataGridViewAppInfo.DataSource = new BindingList<ModuleInfo>(_launcherController.Modules.ToList());
        }

        private void LauncherController_MonitoringChanged(object? sender, EventArgs e)
        {
            ConfigureView();
        }

        private void LauncherController_LauncherConfigurationStatusChanged(object? sender, EventArgs e)
        {
            ConfigureView();
            dataGridViewAppInfo.Invalidate();
        }

        private void ConfigureForm()
        {
            Icon = Resources.LauncherIcon;
            newToolStripMenuItem.Image = Resources.NewFile_16x;
            openToolStripMenuItem.Image = Resources.OpenFile_16x;
            saveToolStripMenuItem.Image = Resources.Save_16x;
            saveAsToolStripMenuItem.Image = Resources.SaveAs_16x;
            settingsToolStripMenuItem.Image = Resources.Settings_16x;
            buttonRunChecked.Image = Resources.RunChecked_16x;
            buttonStopAll.Image = Resources.Stop_16x;
            buttonMoveDown.Image = Resources.CollapseChevronDown_16x;
            buttonMoveUp.Image = Resources.CollapseChevronUp_16x;
            buttonAdd.Image = Resources.Add_16x;
            buttonEdit.Image = Resources.Edit_16x;
            buttonRemove.Image = Resources.Remove_16x;
            buttonStartStopMonitoring.Image = Resources.Run_16x;
            startToolStripMenuItem.Image = Resources.Run_16x;
            stopToolStripMenuItem.Image = Resources.Stop_16x;
            statusLabelProjectPath.Image = Resources.Document_16x;

            FormClosing += FormMain_FormClosing;

            buttonRunChecked.Click += ButtonRunChecked_Click;
            buttonStopAll.Click += ButtonStopAll_Click;
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonRemove.Click += ButtonRemove_Click;
            buttonMoveUp.Click += ButtonMoveUp_Click;
            buttonMoveDown.Click += ButtonMoveDown_Click;
            buttonStartStopMonitoring.Click += ButtonStartStopMonitoring_Click;

            newToolStripMenuItem.Click += NewToolStripMenuItem_Click;
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            saveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;

            startToolStripMenuItem.Click += StartToolStripMenuItem_Click;
            stopToolStripMenuItem.Click += StopToolStripMenuItem_Click;

            statusLabelProjectPath.DoubleClickEnabled = true;
            statusLabelProjectPath.DoubleClick += StatusLabelProjectPath_DoubleClick;

            ConfigureGrid();
            ConfigureView();
        }

        private void StatusLabelProjectPath_DoubleClick(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(statusLabelProjectPath.Text)
                && File.Exists(statusLabelProjectPath.Text))
            {
                Process.Start("explorer.exe", "/select," + statusLabelProjectPath.Text);
            }
        }

        private void ButtonStartStopMonitoring_Click(object? sender, EventArgs e)
        {
            if (_launcherController.IsMonitoring)
            {
                _launcherController.StopMonitoring();
            }
            else
            {
                _launcherController.StartMonitoring();
            }
        }

        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using FormSettings form = new(_userPreferences);

            if (form.ShowDialog() == DialogResult.OK)
            {
                SaveUserPreferences();
            }
        }

        private void LoadUserPreferences()
        {
            if (File.Exists(_userPreferencesPath) == false)
            {
                string newUserPreferences = JsonConvert.SerializeObject(_userPreferences);
                File.WriteAllText(_userPreferencesPath, newUserPreferences);
            }

            string jsonText = File.ReadAllText(_userPreferencesPath, Encoding.UTF8);
            UserPreferences? userPreferences = JsonConvert.DeserializeObject<UserPreferences>(jsonText);

            if (userPreferences is not null)
            {
                _userPreferences = userPreferences;
                ReloadRecentProjects();
            }
        }

        private void CheckRecentProjects()
        {
            bool changed = false;

            for (int i = 0; i < _userPreferences.RecentProjects.Count; i++)
            {
                string project = _userPreferences.RecentProjects[i];

                if (File.Exists(project) == false)
                {
                    changed = true;
                    _userPreferences.RecentProjects.Remove(project);
                    i--;
                }
            }

            if (changed)
            {
                SaveUserPreferences();
            }
        }

        private void ReloadRecentProjects()
        {
            CheckRecentProjects();

            recentProjectsToolStripMenuItem.Enabled = false;
            recentProjectsToolStripMenuItem.DropDownItems.Clear();

            foreach (string project in _userPreferences.RecentProjects)
            {
                ToolStripMenuItem toolStripMenuItem = new();
                toolStripMenuItem.Text = project;
                toolStripMenuItem.Image = Resources.Document_16x;
                toolStripMenuItem.Click += ToolStripMenuItem_Click;
                recentProjectsToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
            }

            if (_userPreferences.RecentProjects.Any())
            {
                ToolStripSeparator separator = new();
                recentProjectsToolStripMenuItem.DropDownItems.Insert(0, separator);

                ToolStripMenuItem clearToolStripMenuItem = new();
                clearToolStripMenuItem.Text = "Clear History";
                clearToolStripMenuItem.Image = Resources.CleanData_16x;
                clearToolStripMenuItem.Click += ClearToolStripMenuItem_Click;
                recentProjectsToolStripMenuItem.DropDownItems.Insert(0, clearToolStripMenuItem);

                recentProjectsToolStripMenuItem.Enabled = true;
            }
        }

        private void ClearToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete the recent projects history?",
                "Warning",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                _userPreferences?.RecentProjects.Clear();
                SaveUserPreferences();
                ReloadRecentProjects();
            }
        }

        private void ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not null && sender is ToolStripMenuItem menuItem)
            {
                if (File.Exists(menuItem.Text) == false)
                {
                    ReloadRecentProjects();
                    return;
                }

                if (_launcherController.UnsavedChanges)
                {
                    DialogResult result = MessageBox.Show(
                        "The current project has unsaved changes. Are you sure you want to proceed and discard the changes?",
                        "Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                LoadProject(menuItem.Text);
            }
        }

        private async void StopToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewAppInfo.CurrentRow != null
                && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                if (moduleInfo.ModuleStatus == ModuleStatus.Running)
                {
                    await _launcherController.StopModuleAsync(moduleInfo);
                }
            }
        }

        private async void StartToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (dataGridViewAppInfo.CurrentRow != null
                && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                if (moduleInfo.ModuleStatus == ModuleStatus.Stopped
                    || (moduleInfo.IsService == false && moduleInfo.AllowMultipleInstances))
                {
                    await _launcherController.StartModuleAsync(moduleInfo);
                }
            }
        }

        private void FormMain_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_launcherController.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show(
                    "The current project has unsaved changes. Are you sure you want to leave and discard the changes?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void NewToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_launcherController.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show(
                    "The current project has unsaved changes. Are you sure you want to proceed and discard the changes?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            CreateNewProject();
            ConfigureView();
        }

        private void ConfigureView()
        {
            Invoke(() =>
            {
                Text = $"{(_launcherController.UnsavedChanges ? "*" : string.Empty)}{_launcherController.ProjectName} - Launcher";

                if (!string.IsNullOrWhiteSpace(_launcherController.ProjectPath))
                {
                    statusLabelProjectPath.Text = _launcherController.ProjectPath;
                }
                else
                {
                    statusLabelProjectPath.Text = _launcherController.ProjectName;
                }

                bool allModulesStopped = _launcherController.LauncherConfigurationStatus == LauncherConfigurationStatus.AllModulesStopped;
                bool isMonitoring = _launcherController.IsMonitoring;

                buttonAdd.Enabled = allModulesStopped && !isMonitoring;
                buttonEdit.Enabled = allModulesStopped && !isMonitoring;
                buttonRemove.Enabled = allModulesStopped && !isMonitoring;
                buttonMoveUp.Enabled = allModulesStopped && !isMonitoring;
                buttonMoveDown.Enabled = allModulesStopped && !isMonitoring;
                buttonStartStopMonitoring.Text = isMonitoring ? "Stop Monitoring" : "Start Monitoring";
                buttonStartStopMonitoring.Image = isMonitoring ? Resources.Stop_16x : Resources.Run_16x;
                saveAsToolStripMenuItem.Enabled = allModulesStopped && !isMonitoring;
                saveToolStripMenuItem.Enabled = allModulesStopped && !isMonitoring;
                openToolStripMenuItem.Enabled = allModulesStopped && !isMonitoring;
                newToolStripMenuItem.Enabled = allModulesStopped && !isMonitoring;

                recentProjectsToolStripMenuItem.Enabled = allModulesStopped && !isMonitoring
                    && recentProjectsToolStripMenuItem.DropDownItems.Count > 0;
            });
        }

        private void SaveToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_launcherController.ProjectPath) is false)
            {
                _launcherController.SaveToDisk();
            }
            else
            {
                SaveAsToolStripMenuItem_Click(sender, e);
            }

            ConfigureView();
        }

        private async void ButtonStopAll_Click(object? sender, EventArgs e)
        {
            buttonStopAll.Enabled = false;
            buttonRunChecked.Enabled = false;

            await _launcherController.StopAllAsync();

            dataGridViewAppInfo.Invalidate();

            buttonStopAll.Enabled = true;
            buttonRunChecked.Enabled = true;

            ConfigureView();
        }

        private void ButtonMoveDown_Click(object? sender, EventArgs e)
        {
            if (dataGridViewAppInfo.CurrentRow is not null
                && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                buttonMoveDown.Enabled = false;

                RemoveDataGridViewEvents();

                int newIndex = _launcherController.MoveDown(moduleInfo);

                AddDataGridViewEvents();

                dataGridViewAppInfo.Invalidate();
                dataGridViewAppInfo.SelectRow(newIndex);

                buttonMoveDown.Enabled = true;

                ConfigureView();

                buttonMoveDown.Focus();
            }
        }

        private void ButtonMoveUp_Click(object? sender, EventArgs e)
        {
            if (dataGridViewAppInfo.CurrentRow is not null
                && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                buttonMoveUp.Enabled = false;

                RemoveDataGridViewEvents();

                int newIndex = _launcherController.MoveUp(moduleInfo);

                AddDataGridViewEvents();

                dataGridViewAppInfo.Invalidate();
                dataGridViewAppInfo.SelectRow(newIndex);

                buttonMoveUp.Enabled = true;

                ConfigureView();

                buttonMoveUp.Focus();
            }
        }

        private void OpenToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "JSON Files | *.json",
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadProject(openFileDialog.FileName);
            }

            ConfigureView();
        }

        private void LoadProject(string fileName)
        {
            LauncherConfiguration? launcherConfiguration = LauncherConfiguration.LoadFromFile(fileName);

            if (launcherConfiguration is not null)
            {
                AddRecentProject(fileName);

                _launcherController = new(
                    launcherConfiguration,
                    fileName,
                    Path.GetFileNameWithoutExtension(fileName));

                _launcherController.ModuleInfoChanged += LauncherController_ModuleInfoChanged;
                _launcherController.LauncherConfigurationStatusChanged += LauncherController_LauncherConfigurationStatusChanged;
                _launcherController.MonitoringChanged += LauncherController_MonitoringChanged;

                dataGridViewAppInfo.DataSource = launcherConfiguration.Modules;

                ConfigureView();
            }
            else
            {
                // Assign an empty list instead of null, so the
                // grid keeps the column definitions.
                dataGridViewAppInfo.DataSource = new BindingList<ModuleInfo>();

                CreateNewProject();

                MessageBox.Show(
                    "Unable to load the json file.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LauncherController_ModuleInfoChanged(object? sender, EventArgs e)
        {
            ConfigureView();
            dataGridViewAppInfo.Invalidate();
        }

        private void SaveAsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "JSON Files | *.json",
                DefaultExt = ".json",
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _launcherController.SaveToDisk(
                    saveFileDialog.FileName,
                    Path.GetFileNameWithoutExtension(saveFileDialog.FileName));

                AddRecentProject(saveFileDialog.FileName);
            }

            ConfigureView();
        }

        private void AddRecentProject(string fileName)
        {
            if (_userPreferences is null)
            {
                return;
            }

            if (_userPreferences.RecentProjects.Contains(fileName))
            {
                _userPreferences.RecentProjects.Remove(fileName);
            }

            _userPreferences.RecentProjects.Insert(0, fileName);

            SaveUserPreferences();
            ReloadRecentProjects();
        }

        private void ConfigureGrid()
        {
            dataGridViewAppInfo.DataSource = new BindingList<ModuleInfo>(_launcherController.Modules.ToList());
            dataGridViewAppInfo.Configure();
            dataGridViewAppInfo.ShowCellToolTips = true;
            dataGridViewAppInfo.ReadOnly = false;
            dataGridViewAppInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.RunOnStartAll)].HeaderText = string.Empty;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.RunOnStartAll)].ToolTipText = "Run";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.RunOnStartAll)].DisplayIndex = 0;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.RunOnStartAll)].MinimumWidth = 30;

            DataGridViewImageColumn moduleTypeImageColumn = new();
            moduleTypeImageColumn.Name = "moduleTypeImageColumn";
            moduleTypeImageColumn.Image = Resources.Application_16x;
            moduleTypeImageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            moduleTypeImageColumn.MinimumWidth = 30;
            moduleTypeImageColumn.HeaderText = string.Empty;
            moduleTypeImageColumn.ToolTipText = "Module Type";
            dataGridViewAppInfo.Columns.Insert(1, moduleTypeImageColumn);

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.DisplayName)].ReadOnly = true;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.DisplayName)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.DisplayName)].HeaderText = "Module Display Name";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.DisplayName)].DisplayIndex = 2;

            DataGridViewImageColumn statusImageColumn = new();
            statusImageColumn.Name = "statusImageColumn";
            statusImageColumn.Image = Resources.StatusStop_16x;
            statusImageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            statusImageColumn.MinimumWidth = 30;
            statusImageColumn.HeaderText = string.Empty;
            statusImageColumn.ToolTipText = "Status";
            dataGridViewAppInfo.Columns.Insert(3, statusImageColumn);

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.IsService)].HeaderText = string.Empty;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.IsService)].ToolTipText = "Is Service";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.IsService)].ReadOnly = true;

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.CpuUsage)].HeaderText = "CPU";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.CpuUsage)].ToolTipText = "CPU";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.CpuUsage)].ReadOnly = true;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.CpuUsage)].DefaultCellStyle.Format = "#0.#\\%";

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.MemoryUsage)].HeaderText = "Memory";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.MemoryUsage)].ToolTipText = "Memory";
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.MemoryUsage)].ReadOnly = true;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.MemoryUsage)].DefaultCellStyle.Format = "#0.#\\ \\M\\B";

            dataGridViewAppInfo.Columns[nameof(ModuleInfo.Index)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.Path)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.ServiceController)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.ModuleStatus)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.IsService)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.AllowMultipleInstances)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.Arguments)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.PreviousCpuUsageTime)].Visible = false;
            dataGridViewAppInfo.Columns[nameof(ModuleInfo.PreviousTotalProcessorTime)].Visible = false;

            dataGridViewAppInfo.CellMouseDown += DataGridViewAppInfo_CellMouseDown;

            AddDataGridViewEvents();
        }

        private void DataGridViewAppInfo_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridViewAppInfo.SelectRow(e.RowIndex);

                if (dataGridViewAppInfo.CurrentRow != null
                    && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
                {
                    if (moduleInfo.IsService == false && moduleInfo.AllowMultipleInstances)
                    {
                        startToolStripMenuItem.Text = "Start New";
                        stopToolStripMenuItem.Text = "Stop";

                        contextMenuProcessesSeparator.Visible = false;
                        processesToolStripMenuItem.Visible = false;
                        processesToolStripMenuItem.DropDownItems.Clear();

                        if (moduleInfo.ProcessIds.Count > 1)
                        {
                            contextMenuProcessesSeparator.Visible = true;
                            processesToolStripMenuItem.Visible = true;
                            stopToolStripMenuItem.Text = "Stop All";

                            for (int i = 0; i < moduleInfo.ProcessIds.Count; i++)
                            {
                                int processId = moduleInfo.ProcessIds[i];

                                ToolStripMenuItem showWindowMenuItem = new();
                                showWindowMenuItem.Text = $"{i}: Show Window of Process {processId}";
                                showWindowMenuItem.Image = Resources.Application_16x;
                                showWindowMenuItem.Tag = processId;
                                showWindowMenuItem.Click += ShowWindowMenuItem_Click;
                                processesToolStripMenuItem.DropDownItems.Add(showWindowMenuItem);

                                ToolStripMenuItem stopProcessMenuItem = new();
                                stopProcessMenuItem.Text = $"{i}: Stop Process {processId}";
                                stopProcessMenuItem.Image = Resources.Stop_16x;
                                stopProcessMenuItem.Tag = processId;
                                stopProcessMenuItem.Click += StopProcessMenuItem_Click;
                                processesToolStripMenuItem.DropDownItems.Add(stopProcessMenuItem);
                            }
                        }
                    }
                    else
                    {
                        contextMenuProcessesSeparator.Visible = false;
                        processesToolStripMenuItem.Visible = false;
                        processesToolStripMenuItem.DropDownItems.Clear();

                        startToolStripMenuItem.Text = "Start";
                        stopToolStripMenuItem.Text = "Stop";
                    }

                    stopToolStripMenuItem.Enabled = moduleInfo.ModuleStatus == ModuleStatus.Running;
                    startToolStripMenuItem.Enabled = moduleInfo.ModuleStatus == ModuleStatus.Stopped
                        || (moduleInfo.IsService == false && moduleInfo.AllowMultipleInstances);
                }
            }
        }

        private async void StopProcessMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not null
                && sender is ToolStripMenuItem menuItem
                && menuItem.Tag is not null
                && menuItem.Tag is int processId)
            {
                await _launcherController.StopProcessAsync(processId);
            }
        }

        private void ShowWindowMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not null
                && sender is ToolStripMenuItem menuItem
                && menuItem.Tag is not null
                && menuItem.Tag is int processId)
            {
                Process? process = Process.GetProcessById(processId);

                if (process is not null && process.MainWindowHandle.ToInt32() != 0)
                {
                    ShowWindowAsync(process.MainWindowHandle, SW_SHOWNORMAL);
                    SetForegroundWindow(process.MainWindowHandle);
                }
            }
        }

        private void AddDataGridViewEvents()
        {
            dataGridViewAppInfo.CellFormatting += DataGridViewAppInfo_CellFormatting;
            dataGridViewAppInfo.CellDoubleClick += DataGridViewAppInfo_CellDoubleClick;
        }

        private void RemoveDataGridViewEvents()
        {
            dataGridViewAppInfo.CellFormatting -= DataGridViewAppInfo_CellFormatting;
            dataGridViewAppInfo.CellDoubleClick -= DataGridViewAppInfo_CellDoubleClick;
        }

        private void DataGridViewAppInfo_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            EditModule();
        }

        private void DataGridViewAppInfo_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridViewAppInfo.Columns[e.ColumnIndex].Name == "statusImageColumn")
            {
                DataGridViewRow row = dataGridViewAppInfo.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                if ((ModuleStatus)row.Cells[nameof(ModuleInfo.ModuleStatus)].Value == ModuleStatus.Running)
                {
                    e.Value = _imageStatusRun16x;
                    cell.ToolTipText = "Running";
                }
                else
                {
                    e.Value = _imageStatusStop16x;
                    cell.ToolTipText = "Stopped";
                }
            }
            else if (dataGridViewAppInfo.Columns[e.ColumnIndex].Name == "moduleTypeImageColumn")
            {
                if (dataGridViewAppInfo.Rows[e.RowIndex].DataBoundItem is ModuleInfo moduleInfo)
                {
                    DataGridViewRow row = dataGridViewAppInfo.Rows[e.RowIndex];
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];

                    if (moduleInfo.IsService)
                    {
                        e.Value = _imageWindowsService16x;
                        cell.ToolTipText = "Windows Service";
                    }
                    else if (moduleInfo.Path is not null && moduleInfo.Path.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Value = _imageBatchFile16x;
                        cell.ToolTipText = "Batch Script";
                    }
                    else
                    {
                        e.Value = _imageApplication16x;
                        cell.ToolTipText = "Application";
                    }
                }
            }
            else if (dataGridViewAppInfo.Columns[e.ColumnIndex].Name == nameof(ModuleInfo.CpuUsage))
            {
                if (dataGridViewAppInfo.Rows[e.RowIndex].DataBoundItem is ModuleInfo moduleInfo)
                {
                    if (moduleInfo.CpuUsage == -1)
                    {
                        e.Value = "--";
                    }
                }
            }
            else if (dataGridViewAppInfo.Columns[e.ColumnIndex].Name == nameof(ModuleInfo.MemoryUsage))
            {
                if (dataGridViewAppInfo.Rows[e.RowIndex].DataBoundItem is ModuleInfo moduleInfo)
                {
                    if (moduleInfo.MemoryUsage == -1)
                    {
                        e.Value = "--";
                    }
                }
            }
        }

        private void ButtonRemove_Click(object? sender, EventArgs e)
        {
            if (dataGridViewAppInfo.CurrentRow is not null
                && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Are you sure you want to delete the selected record?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (dialogResult == DialogResult.Yes)
                {
                    _launcherController.Remove(moduleInfo);
                    ConfigureView();
                }
            }
        }

        private void ButtonEdit_Click(object? sender, EventArgs e)
        {
            EditModule();
        }

        private void EditModule()
        {
            if (_launcherController.AnyModuleRunning() || _launcherController.IsMonitoring)
            {
                return;
            }

            if (dataGridViewAppInfo.CurrentRow is not null && dataGridViewAppInfo.CurrentRow.DataBoundItem is ModuleInfo moduleInfo)
            {
                using FormEditModuleInfo form = new(moduleInfo);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewAppInfo.Invalidate();

                    if (form.ModuleInfoChanged)
                    {
                        _launcherController.SetUnsavedChanges();
                    }
                }

                ConfigureView();
            }
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            using FormEditModuleInfo form = new(null);

            if (form.ShowDialog() == DialogResult.OK)
            {
                _launcherController.Add(form.ModuleInfo);
                dataGridViewAppInfo.Invalidate();
                ConfigureView();
            }
        }

        private async void ButtonRunChecked_Click(object? sender, EventArgs e)
        {
            buttonRunChecked.Enabled = false;
            buttonStopAll.Enabled = false;
            buttonAdd.Enabled = false;
            buttonEdit.Enabled = false;
            buttonRemove.Enabled = false;
            buttonMoveUp.Enabled = false;
            buttonMoveDown.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = false;
            newToolStripMenuItem.Enabled = false;

            await _launcherController.StartAllAsync();

            dataGridViewAppInfo.Invalidate();

            buttonRunChecked.Enabled = true;
            buttonStopAll.Enabled = true;

            ConfigureView();
        }
    }
}