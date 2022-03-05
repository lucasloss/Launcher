using Microsoft.Win32;
using System.Diagnostics;
using System.ServiceProcess;

namespace Launcher.Core
{
    /// <summary>
    /// Provide tools to start, monitor and stop Windows applications, Windows services and batch files.
    /// </summary>
    public class LauncherController
    {
        private readonly HashSet<string> _processIgnoreList = new();
        private readonly List<ProcessContainer> _processContainers = new();
        private readonly LauncherConfiguration _launcherConfiguration;
        private readonly System.Timers.Timer _timer = new();
        private bool _timerStopRequested = false;
        private bool _unsavedChanges = false;
        private LauncherConfigurationStatus _launcherConfigurationStatus = LauncherConfigurationStatus.AllModulesStopped;
        private DateTime _nextRefresh = DateTime.MinValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherController"/> class.
        /// </summary>
        public LauncherController()
        {
            _launcherConfiguration = new LauncherConfiguration();
            ProjectPath = string.Empty;
            ProjectName = "New Project";

            Configure();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherController"/> class.
        /// </summary>
        /// <param name="launcherConfiguration">The <see cref="LauncherConfiguration"/> to be controlled by the <see cref="LauncherController"/>.</param>
        /// <param name="projectPath">The path of the project file that represents the <see cref="LauncherConfiguration"/> on disk.</param>
        /// <param name="projectName">The name of the project.</param>
        public LauncherController(LauncherConfiguration launcherConfiguration, string projectPath, string projectName)
        {
            _launcherConfiguration = launcherConfiguration ?? throw new ArgumentNullException(nameof(launcherConfiguration));
            ProjectPath = projectPath;
            ProjectName = projectName;

            Configure();
        }

        /// <summary>
        /// Notifies when at least one <see cref="ModuleInfo"/> has been changed.
        /// </summary>
        public event EventHandler? ModuleInfoChanged;

        /// <summary>
        /// Notifies when the status of the <see cref="LauncherConfiguration"/> has changed.
        /// </summary>
        public event EventHandler? LauncherConfigurationStatusChanged;

        /// <summary>
        /// Notifies when the status of the <see cref="IsMonitoring"/> property has changed.
        /// </summary>
        public event EventHandler? MonitoringChanged;

        /// <summary>
        /// Gets the list of <see cref="ModuleInfo"/> associated with the <see cref="LauncherConfiguration"/>.
        /// </summary>
        public IEnumerable<ModuleInfo> Modules => _launcherConfiguration.Modules;

        /// <summary>
        /// Gets the status of the <see cref="LauncherConfiguration"/>.
        /// </summary>
        public LauncherConfigurationStatus LauncherConfigurationStatus
        {
            get => _launcherConfigurationStatus;

            private set
            {
                bool raiseEvent = false;

                if (_launcherConfigurationStatus != value)
                {
                    raiseEvent = true;
                }

                _launcherConfigurationStatus = value;

                if (raiseEvent)
                {
                    OnLauncherConfigurationStatusChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the project path, including file name and extension.
        /// </summary>
        public string ProjectPath { get; private set; }

        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; private set; } = "New Project";

        /// <summary>
        /// Gets a value indicating whether the modules are being monitored.
        /// When <see langword="true"/>, operations that change the list of modules are not allowed.
        /// </summary>
        public bool IsMonitoring { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="LauncherConfiguration"/> has unsaved changes.
        /// </summary>
        public bool UnsavedChanges => _unsavedChanges;

        /// <summary>
        /// Start the monitoring process.
        /// </summary>
        public void StartMonitoring()
        {
            _timer.Start();
            _timerStopRequested = false;
            IsMonitoring = true;
            OnMonitoringChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Stop the monitoring process.
        /// </summary>
        public void StopMonitoring()
        {
            _timerStopRequested = true;
            _timer.Stop();
            IsMonitoring = false;
            OnMonitoringChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets a value indicating whether there is at least one module running.
        /// </summary>
        /// <returns><see langword="true"/> if there is at least one module running; otherwise, <see langword="false"/>.</returns>
        public bool AnyModuleRunning()
        {
            return _launcherConfiguration.Modules.Any(m => m.ModuleStatus == ModuleStatus.Running);
        }

        /// <summary>
        /// Iterates through the list of modules and updates the Status, CPU Usage and Memory Usage.
        /// This method raises the ModuleInfoChanged event when at least one module is updated.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<long> RefreshModulesInformation()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool changed = false;

            await Task.Run(() =>
            {
                // Do not refresh while starting one or more modules.
                if (LauncherConfigurationStatus == LauncherConfigurationStatus.StartAllRequested
                    || LauncherConfigurationStatus == LauncherConfigurationStatus.StartModuleRequested)
                {
                    return;
                }

                List<ModuleInfo> modulesRunning = _launcherConfiguration.Modules
                    .Where(m => m.ModuleStatus == ModuleStatus.Running && m.ProcessIds.Any())
                    .ToList();

                foreach (ModuleInfo moduleInfo in modulesRunning)
                {
                    if (RemoveDeadProcesses(moduleInfo))
                    {
                        changed = true;
                    }

                    if (moduleInfo.ProcessIds.Count == 1)
                    {
                        Process? process = GetProcessById(moduleInfo.ProcessIds.First());

                        if (process is not null)
                        {
                            DateTime previousCpuUsageTime = moduleInfo.PreviousCpuUsageTime;
                            TimeSpan previousTotalProcessorTime = moduleInfo.PreviousTotalProcessorTime;

                            moduleInfo.CpuUsage = GetProcessCpuUsage(
                                process,
                                ref previousCpuUsageTime,
                                ref previousTotalProcessorTime);

                            moduleInfo.PreviousCpuUsageTime = previousCpuUsageTime;
                            moduleInfo.PreviousTotalProcessorTime = previousTotalProcessorTime;
                            moduleInfo.MemoryUsage = process.PrivateMemorySize64 / 1024.0 / 1024.0;
                            changed = true;
                        }
                    }
                    else if (moduleInfo.ProcessIds.Count > 1)
                    {
                        moduleInfo.CpuUsage = -1;
                        moduleInfo.PreviousCpuUsageTime = DateTime.MinValue;
                        moduleInfo.PreviousTotalProcessorTime = TimeSpan.Zero;
                        moduleInfo.MemoryUsage = -1;
                        changed = true;
                    }
                    else
                    {
                        moduleInfo.CpuUsage = 0;
                        moduleInfo.PreviousCpuUsageTime = DateTime.MinValue;
                        moduleInfo.PreviousTotalProcessorTime = TimeSpan.Zero;
                        moduleInfo.MemoryUsage = 0;
                        changed = true;
                    }
                }

                UpdateMainProcessList();

                List<ModuleInfo> modulesNotRunning = _launcherConfiguration.Modules.Where(
                    m => m.ModuleStatus != ModuleStatus.Running
                    && m.Path is not null
                    && !m.Path.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase)).ToList();

                foreach (ModuleInfo moduleInfo in modulesNotRunning)
                {
                    if (moduleInfo.ProcessIds.Any())
                    {
                        Process? process = GetProcessById(moduleInfo.ProcessIds.First());

                        if (process is null || process.HasExited)
                        {
                            if (process is not null)
                            {
                                moduleInfo.ProcessIds.Remove(process.Id);
                            }

                            moduleInfo.Stop();
                            changed = true;
                        }
                    }
                    else
                    {
                        try
                        {
                            for (int i = 0; i < _processContainers.Count; i++)
                            {
                                ProcessContainer processContainer = _processContainers[i];

                                if (processContainer.Process.HasExited is false
                                    && processContainer.FileName == moduleInfo.Path)
                                {
                                    moduleInfo.ProcessIds.Add(processContainer.Process.Id);
                                    moduleInfo.ModuleStatus = ModuleStatus.Running;
                                    changed = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            moduleInfo.ProcessIds.Clear();
                            moduleInfo.ModuleStatus = ModuleStatus.Stopped;
                        }
                    }
                }
            });

            if (changed)
            {
                UpdateLauncherConfigurationStatusProperty();
                OnModuleInfoChanged(EventArgs.Empty);
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Adds a new <see cref="ModuleInfo"/> to the project.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be added to the project.</param>
        public void Add(ModuleInfo moduleInfo)
        {
            if (IsMonitoring)
            {
                throw new InvalidOperationException("Unable to modify the list of modules while the " +
                    "monitoring process is running. Stop the monitoring process and try again.");
            }

            _launcherConfiguration.Add(moduleInfo);
            ConfigureServiceController(moduleInfo);
            _unsavedChanges = true;
            OnModuleInfoChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Removes a <see cref="ModuleInfo"/> from the project.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be removed.</param>
        public void Remove(ModuleInfo moduleInfo)
        {
            if (IsMonitoring)
            {
                throw new InvalidOperationException("Unable to modify the list of modules while the " +
                    "monitoring process is running. Stop the monitoring process and try again.");
            }

            _launcherConfiguration.Remove(moduleInfo);
            _unsavedChanges = true;
            OnModuleInfoChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Sets the <see cref="UnsavedChanges"/> property to true.
        /// </summary>
        public void SetUnsavedChanges()
        {
            _unsavedChanges = true;
        }

        /// <summary>
        /// Saves the <see cref="LauncherConfiguration"/> to disk.
        /// </summary>
        public void SaveToDisk()
        {
            _launcherConfiguration.SaveToDisk(ProjectPath);
            _unsavedChanges = false;
        }

        /// <summary>
        /// Saves the <see cref="LauncherConfiguration"/> to disk.
        /// </summary>
        /// <param name="projectPath">The path where the project should be saved, including file name and extension.</param>
        /// <param name="projectName">The project name.</param>
        public void SaveToDisk(string projectPath, string projectName)
        {
            ProjectPath = projectPath;
            ProjectName = projectName;
            SaveToDisk();
        }

        /// <summary>
        /// Stops all running modules.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopAllAsync()
        {
            LauncherConfigurationStatus = LauncherConfigurationStatus.StopAllRequested;

            foreach (ModuleInfo moduleInfo in _launcherConfiguration.Modules
                .Where(m => m.ModuleStatus == ModuleStatus.Running)
                .OrderByDescending(m => m.Index))
            {
                if (moduleInfo.IsService && moduleInfo.ServiceController is not null)
                {
                    await StopServiceAsync(moduleInfo);
                }
                else
                {
                    await StopAppAsync(moduleInfo);
                }
            }

            UpdateLauncherConfigurationStatusProperty();
        }

        /// <summary>
        /// Stops the execution of a single <see cref="ModuleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be stopped.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopModuleAsync(ModuleInfo moduleInfo)
        {
            LauncherConfigurationStatus = LauncherConfigurationStatus.StopModuleRequested;

            if (moduleInfo.IsService && moduleInfo.ServiceController is not null)
            {
                await StopServiceAsync(moduleInfo);
            }
            else
            {
                await StopAppAsync(moduleInfo);
            }

            UpdateLauncherConfigurationStatusProperty();
        }

        /// <summary>
        /// Start all modules where the <see cref="ModuleInfo.RunOnStartAll"/> property is <see langword="true"/>.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartAllAsync()
        {
            LauncherConfigurationStatus = LauncherConfigurationStatus.StartAllRequested;

            ConfigureServiceControllers();

            await RefreshModulesInformation();

            foreach (ModuleInfo moduleInfo in _launcherConfiguration.Modules.Where(
                m => m.RunOnStartAll
                && m.ModuleStatus == ModuleStatus.Stopped
                && File.Exists(m.Path)))
            {
                if (moduleInfo.IsService && moduleInfo.ServiceController is not null)
                {
                    await StartServiceAsync(moduleInfo);
                }
                else if (moduleInfo.Path is not null && moduleInfo.Path.EndsWith(".bat", StringComparison.InvariantCultureIgnoreCase))
                {
                    await RunBatchScriptAsync(moduleInfo);
                }
                else
                {
                    StartApp(moduleInfo);
                }
            }

            UpdateLauncherConfigurationStatusProperty();
        }

        /// <summary>
        /// Start a single module.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be started.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartModuleAsync(ModuleInfo moduleInfo)
        {
            if (File.Exists(moduleInfo.Path) is false)
            {
                return;
            }

            LauncherConfigurationStatus = LauncherConfigurationStatus.StartModuleRequested;

            ConfigureServiceControllers();

            if (moduleInfo.IsService && moduleInfo.ServiceController is not null)
            {
                await StartServiceAsync(moduleInfo);
            }
            else
            {
                StartApp(moduleInfo);
            }

            UpdateLauncherConfigurationStatusProperty();
        }

        /// <summary>
        /// Moves the <see cref="ModuleInfo"/> up in the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be moved up.</param>
        /// <returns>The new index of the <see cref="ModuleInfo"/> in the list.</returns>
        public int MoveUp(ModuleInfo moduleInfo)
        {
            if (IsMonitoring)
            {
                throw new InvalidOperationException("Unable to modify the list of modules while the " +
                    "monitoring process is running. Stop the monitoring process and try again.");
            }

            int oldIndex = _launcherConfiguration.Modules.IndexOf(moduleInfo);
            int newIndex = _launcherConfiguration.MoveUp(moduleInfo);

            if (oldIndex != newIndex)
            {
                _unsavedChanges = true;
                OnModuleInfoChanged(EventArgs.Empty);
            }

            return newIndex;
        }

        /// <summary>
        /// Moves the <see cref="ModuleInfo"/> down in the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be moved down.</param>
        /// <returns>The new index of the <see cref="ModuleInfo"/> in the list.</returns>
        public int MoveDown(ModuleInfo moduleInfo)
        {
            if (IsMonitoring)
            {
                throw new InvalidOperationException("Unable to modify the list of modules while the " +
                    "monitoring process is running. Stop the monitoring process and try again.");
            }

            int oldIndex = _launcherConfiguration.Modules.IndexOf(moduleInfo);
            int newIndex = _launcherConfiguration.MoveDown(moduleInfo);

            if (oldIndex != newIndex)
            {
                _unsavedChanges = true;
                OnModuleInfoChanged(EventArgs.Empty);
            }

            return newIndex;
        }

        /// <summary>
        /// Configure the service controllers and modules status.
        /// This method should be called every time a new <see cref="LauncherController"/> is created.
        /// </summary>
        public void Configure()
        {
            if (IsMonitoring)
            {
                throw new InvalidOperationException("Unable to modify the list of modules while the " +
                    "monitoring process is running. Stop the monitoring process and try again.");
            }

            ConfigureServiceControllers();
            ConfigureTimer();
        }

        /// <summary>
        /// Stop a <see cref="Process"/> by its process id.
        /// </summary>
        /// <param name="processId">The process id.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StopProcessAsync(int processId)
        {
            ModuleInfo? module = Modules.FirstOrDefault(m => m.ProcessIds.Contains(processId));

            if (module is not null)
            {
                Process process = Process.GetProcessById(processId);

                process.Kill(true);
                await process.WaitForExitAsync();

                module.ProcessIds.Remove(processId);
                module.Stop();
            }
        }

        /// <summary>
        /// Invokes the <see cref="ModuleInfoChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        protected void OnModuleInfoChanged(EventArgs e)
        {
            EventHandler? handler = ModuleInfoChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes the <see cref="LauncherConfigurationStatusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        protected void OnLauncherConfigurationStatusChanged(EventArgs e)
        {
            EventHandler? handler = LauncherConfigurationStatusChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes the <see cref="MonitoringChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        protected void OnMonitoringChanged(EventArgs e)
        {
            EventHandler? handler = MonitoringChanged;
            handler?.Invoke(this, e);
        }

        private static string? GetServiceInstallPath(string serviceName)
        {
            RegistryKey? regkey = Registry.LocalMachine.OpenSubKey(string.Format(@"SYSTEM\CurrentControlSet\services\{0}", serviceName));

            if (regkey is not null && regkey.GetValue("ImagePath") is not null)
            {
                return regkey.GetValue("ImagePath") as string;
            }

            return null;
        }

        private static async Task StopAppAsync(ModuleInfo moduleInfo)
        {
            foreach (int processId in moduleInfo.ProcessIds)
            {
                try
                {
                    Process process = Process.GetProcessById(processId);
                    process.Kill(true);
                    await process.WaitForExitAsync();
                }
                catch
                {
                }
            }

            moduleInfo.ProcessIds.Clear();
            moduleInfo.Stop();
        }

        private static void ConfigureServiceController(ModuleInfo moduleInfo)
        {
            ServiceController[] scServices = ServiceController.GetServices();

            foreach (ServiceController serviceController in scServices)
            {
                string? servicePath = GetServiceInstallPath(serviceController.ServiceName);

                if (servicePath is not null)
                {
                    if (moduleInfo is not null && servicePath == moduleInfo.Path)
                    {
                        moduleInfo.ServiceController = serviceController;
                    }
                }
            }
        }

        private static double GetProcessCpuUsage(Process process, ref DateTime previousTime, ref TimeSpan previousTotalProcessorTime)
        {
            // Original source:
            // https://github.com/jackowild/CpuUsagePercentageDotNetCoreExample
            DateTime now = DateTime.UtcNow;
            TimeSpan totalProcessorTimeNow = process.TotalProcessorTime;

            double cpuUsedMs = (totalProcessorTimeNow - previousTotalProcessorTime).TotalMilliseconds;
            double totalMsPassed = (now - previousTime).TotalMilliseconds;

            double cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

            previousTime = now;
            previousTotalProcessorTime = totalProcessorTimeNow;

            return cpuUsageTotal * 100;
        }

        private static Process? GetProcessById(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool RemoveDeadProcesses(ModuleInfo moduleInfo)
        {
            bool changed = false;

            for (int i = 0; i < moduleInfo.ProcessIds.Count; i++)
            {
                int processId = moduleInfo.ProcessIds[i];
                Process? process = GetProcessById(processId);

                if (process is null || process.HasExited)
                {
                    moduleInfo.ProcessIds.RemoveAt(i);
                    i--;

                    moduleInfo.Stop();
                }

                changed = true;
            }

            return changed;
        }

        private void ConfigureTimer()
        {
            _timer.AutoReset = false;
            _timer.Interval = 100;
            _timer.Elapsed += Timer_Elapsed;
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_timerStopRequested is false)
            {
                _timer.Stop();

                if (DateTime.Now >= _nextRefresh)
                {
                    long elapsedMilliseconds = await RefreshModulesInformation();
                    _nextRefresh = DateTime.Now.AddMilliseconds(1000 - elapsedMilliseconds);
                }

                if (_timerStopRequested is false)
                {
                    _timer.Start();
                }
            }
        }

        private void UpdateLauncherConfigurationStatusProperty()
        {
            if (_launcherConfiguration.Modules.Any(m => m.ModuleStatus == ModuleStatus.Running))
            {
                LauncherConfigurationStatus = LauncherConfigurationStatus.AtLeastOneModuleRunning;
            }
            else
            {
                LauncherConfigurationStatus = LauncherConfigurationStatus.AllModulesStopped;
            }
        }

        /// <summary>
        /// Runs a batch script asynchronous.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> that represents the batch script.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">When the <paramref name="moduleInfo"/> is null.</exception>
        /// <exception cref="ArgumentException">When the <paramref name="moduleInfo"/> is not contained in the list of modules.</exception>
        private async Task RunBatchScriptAsync(ModuleInfo moduleInfo)
        {
            if (File.Exists(moduleInfo.Path) is false)
            {
                return;
            }

            if (moduleInfo is null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

            if (_launcherConfiguration.Modules.Contains(moduleInfo) is false)
            {
                throw new ArgumentException($"The {nameof(ModuleInfo)} specified is not contained in the current {nameof(LauncherConfiguration)}.", nameof(moduleInfo));
            }

            await Task.Run(() =>
            {
                ProcessStartInfo startInfo = new();
                startInfo.FileName = moduleInfo.Path;
                startInfo.Arguments = moduleInfo.Arguments;

                if (moduleInfo.Path is not null)
                {
                    startInfo.WorkingDirectory = Directory.GetParent(moduleInfo.Path)?.FullName;
                }

                Process? process = Process.Start(startInfo);

                if (process is not null)
                {
                    moduleInfo.ProcessIds.Add(process.Id);
                    moduleInfo.ModuleStatus = ModuleStatus.Running;
                    process.WaitForExit();
                }
            });
        }

        private async Task StartServiceAsync(ModuleInfo moduleInfo)
        {
            if (File.Exists(moduleInfo.Path) is false)
            {
                return;
            }

            if (moduleInfo is null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

            if (_launcherConfiguration.Modules.Contains(moduleInfo) is false)
            {
                throw new ArgumentException($"The {nameof(ModuleInfo)} specified is not contained in the current {nameof(LauncherConfiguration)}.", nameof(moduleInfo));
            }

            if (moduleInfo.ServiceController is not null)
            {
                moduleInfo.ServiceController.Refresh();

                if (File.Exists(moduleInfo.Path) && moduleInfo.ServiceController?.Status == ServiceControllerStatus.Stopped)
                {
                    moduleInfo.ServiceController.Start();

                    while (moduleInfo.ServiceController.Status == ServiceControllerStatus.Stopped)
                    {
                        moduleInfo.ServiceController.Refresh();
                        await Task.Delay(10);
                    }

                    moduleInfo.ModuleStatus = ModuleStatus.Running;
                    ProcessContainer? processContainer = await GetProcessByPath(moduleInfo.Path);

                    if (processContainer is not null)
                    {
                        moduleInfo.ProcessIds.Add(processContainer.Process.Id);
                    }
                }
            }
        }

        private void StartApp(ModuleInfo moduleInfo)
        {
            if (File.Exists(moduleInfo.Path) is false)
            {
                return;
            }

            if (moduleInfo is null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

            if (_launcherConfiguration.Modules.Contains(moduleInfo) is false)
            {
                throw new ArgumentException($"The {nameof(ModuleInfo)} specified is not contained in the current {nameof(LauncherConfiguration)}.", nameof(moduleInfo));
            }

            ProcessStartInfo startInfo = new();
            startInfo.FileName = moduleInfo.Path;
            startInfo.Arguments = moduleInfo.Arguments;

            if (moduleInfo.Path is not null)
            {
                startInfo.WorkingDirectory = Directory.GetParent(moduleInfo.Path)?.FullName;
            }

            Process? process = Process.Start(startInfo);

            if (process is not null)
            {
                moduleInfo.ProcessIds.Add(process.Id);
                moduleInfo.ModuleStatus = ModuleStatus.Running;
            }
        }

        private async Task StopServiceAsync(ModuleInfo moduleInfo)
        {
            if (moduleInfo.ServiceController is not null)
            {
                moduleInfo.ServiceController.Refresh();

                if (moduleInfo.ServiceController.Status == ServiceControllerStatus.Running)
                {
                    moduleInfo.ServiceController.Stop(true);

                    try
                    {
                        moduleInfo.ServiceController.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 0, 1000));
                    }
                    catch (System.ServiceProcess.TimeoutException)
                    {
                        await KillProcess(moduleInfo.Path);
                    }

                    moduleInfo.ProcessIds.Clear();
                    moduleInfo.Stop();
                }
            }
        }

        private async Task KillProcess(string? path)
        {
            if (path is not null)
            {
                ProcessContainer? processContainer = await GetProcessByPath(path);

                if (processContainer is not null)
                {
                    processContainer.Process.Kill(true);
                }
            }
        }

        private async Task<ProcessContainer?> GetProcessByPath(string path)
        {
            ProcessContainer? result = await Task.Run(() =>
            {
                UpdateMainProcessList();

                foreach (ProcessContainer processContainer in _processContainers)
                {
                    try
                    {
                        if (processContainer.FileName == path)
                        {
                            return processContainer;
                        }
                    }
                    catch
                    {
                        // Trying to access Process.MainModule sometimes raise exceptions.
                        // These exceptions, in this context, should be ignored.
                    }
                }

                return null;
            });

            return result;
        }

        /// <summary>
        /// Gets a list of processes after removing those processes where
        /// the property <see cref="Process.MainModule"/> is null or inaccessible.
        /// </summary>
        private void UpdateMainProcessList()
        {
            List<ProcessContainer> deadProcesses = _processContainers.Where(p => p.Process.HasExited).ToList();

            foreach (ProcessContainer deadProcessContainer in deadProcesses)
            {
                _processContainers.Remove(deadProcessContainer);
            }

            List<Process> processes = Process.GetProcesses().ToList();
            processes.RemoveAll(p => _processIgnoreList.Contains(p.ProcessName));

            List<string> currentList = _processContainers.Select(p => p.Process.ProcessName).ToList();
            List<string> newList = processes.Select(p => p.ProcessName).ToList();

            List<string> removeList = currentList.Except(newList).ToList();
            List<string> addList = newList.Except(currentList).ToList();

            _processContainers.RemoveAll(p => removeList.Contains(p.Process.ProcessName));

            foreach (string processName in addList)
            {
                Process[] processArray = Process.GetProcessesByName(processName);

                foreach (Process process in processArray)
                {
                    try
                    {
                        // Access to process.MainModule is very expensive
                        // and impacts performance.
                        // Even checking if it is null takes a long time.
                        // And sometimes is it not null and it is not possible
                        // to access MainModule.FileName.
                        // So, to increave performance, I'll disable the
                        // CS8602 warning and not check is MainModule is null.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        // Tries to access the MainModule.
                        // If an exception is raised, the catch below
                        // will remove the process from the list.
                        string? fileName = process.MainModule.FileName;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                        if (fileName is not null)
                        {
                            _processContainers.Add(new ProcessContainer(process, fileName));
                        }
                    }
                    catch
                    {
                        if (_launcherConfiguration.Modules.Any(m => m.Path is not null && m.Path.Contains(process.ProcessName)) == false)
                        {
                            _processIgnoreList.Add(process.ProcessName);
                        }
                    }
                }
            }
        }

        private void ConfigureServiceControllers()
        {
            ServiceController[] scServices = ServiceController.GetServices();

            foreach (ServiceController serviceController in scServices)
            {
                string? servicePath = GetServiceInstallPath(serviceController.ServiceName);

                if (servicePath is not null)
                {
                    ModuleInfo? moduleInfo = _launcherConfiguration.Modules.FirstOrDefault(a => a.Path == servicePath);

                    if (moduleInfo is not null && servicePath == moduleInfo.Path)
                    {
                        moduleInfo.ServiceController = serviceController;
                    }
                }
            }
        }
    }
}