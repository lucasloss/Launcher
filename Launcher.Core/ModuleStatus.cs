namespace Launcher.Core
{
    /// <summary>
    /// Indicates the status of a <see cref="ModuleInfo"/>.
    /// </summary>
    public enum ModuleStatus
    {
        /// <summary>
        /// Indicates that the <see cref="ModuleInfo"/> is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Indicates that the <see cref="ModuleInfo"/> is running.
        /// </summary>
        Running,
    }
}