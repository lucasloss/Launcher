namespace Launcher.Core
{
    /// <summary>
    /// Indicates the status of a <see cref="LauncherConfiguration"/>.
    /// </summary>
    public enum LauncherConfigurationStatus
    {
        /// <summary>
        /// Indicates that the start of all modules has been requested.
        /// </summary>
        StartAllRequested = 0,

        /// <summary>
        /// Indicates that the stop of all modules has been requested.
        /// </summary>
        StopAllRequested = 1,

        /// <summary>
        /// Indicates that at least one modules is running.
        /// </summary>
        AtLeastOneModuleRunning = 2,

        /// <summary>
        /// Indicates that all modules are stopped.
        /// </summary>
        AllModulesStopped = 3,

        /// <summary>
        /// Indicates that the start of a single module has been requested.
        /// </summary>
        StartModuleRequested = 4,

        /// <summary>
        /// Indicates that the stop of a single module has been requested.
        /// </summary>
        StopModuleRequested = 5,
    }
}