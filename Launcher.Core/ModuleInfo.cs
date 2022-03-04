using Newtonsoft.Json;
using System.Diagnostics;
using System.ServiceProcess;

namespace Launcher.Core
{
    /// <summary>
    /// Represents one module that could be a Windows Service, a Windows Application or a Batch Script (.bat).
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// Gets or sets the status of the module.
        /// </summary>
        [JsonIgnore]
        public ModuleStatus ModuleStatus { get; set; }

        /// <summary>
        /// Gets or sets the index of the module in the list.
        /// Used to identify the order in which to run and stop the modules.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module should run when StartAll is called.
        /// </summary>
        public bool RunOnStartAll { get; set; }

        /// <summary>
        /// Gets or sets the display name of the module.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the path of the module, including its name and extension.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the module is a Windows Service.
        /// </summary>
        public bool IsService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether multiple instances of the module are allowed.
        /// </summary>
        public bool AllowMultipleInstances { get; set; }

        /// <summary>
        /// Gets or sets the arguments to be used in the execution of the module.
        /// </summary>
        public string? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ServiceController"/> associated with the module. Used only for Windows Services.
        /// </summary>
        [JsonIgnore]
        public ServiceController? ServiceController { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Process"/> identifiers.
        /// </summary>
        [JsonIgnore]
        public List<int> ProcessIds { get; } = new();

        /// <summary>
        /// Gets or sets the CPU usage for the process.
        /// </summary>
        [JsonIgnore]
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the previous value of <see cref="Process.TotalProcessorTime"/>.
        /// </summary>
        [JsonIgnore]
        public TimeSpan PreviousTotalProcessorTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the last time the CPU usage was calculated.
        /// </summary>
        [JsonIgnore]
        public DateTime PreviousCpuUsageTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the memory usage for the process.
        /// </summary>
        [JsonIgnore]
        public double MemoryUsage { get; set; }

        /// <summary>
        /// When there are no more processes running (<see cref="ProcessIds"/> is empty),
        /// this method:
        /// <list type="bullet">
        /// <item>Sets the <see cref="ModuleStatus"/> property to <see cref="ModuleStatus.Stopped"/></item>
        /// <item>Sets the <see cref="PreviousCpuUsageTime"/> property to <see cref="DateTime.MinValue"/></item>
        /// <item>Sets the <see cref="PreviousTotalProcessorTime"/> property to <see cref="TimeSpan.Zero"/></item>
        /// <item>Sets the <see cref="CpuUsage"/> property to 0</item>
        /// <item>Sets the <see cref="MemoryUsage"/> property to 0</item>
        /// </list>
        /// </summary>
        public void Stop()
        {
            if (ProcessIds.Any() == false)
            {
                ModuleStatus = ModuleStatus.Stopped;
                PreviousCpuUsageTime = DateTime.MinValue;
                PreviousTotalProcessorTime = TimeSpan.Zero;
                CpuUsage = 0;
                MemoryUsage = 0;
            }
        }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return DisplayName;
        }
    }
}