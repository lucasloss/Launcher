namespace Launcher
{
    /// <summary>
    /// Control the user preferences to be saved on disk.
    /// </summary>
    public class UserPreferences
    {
        /// <summary>
        /// Gets or sets a list of recent projects.
        /// </summary>
        public List<string> RecentProjects { get; set; } = new();

        /// <summary>
        /// Gets or sets the default startup project path.
        /// </summary>
        public string? DefaultStartupProjectPath { get; set; } = string.Empty;
    }
}
