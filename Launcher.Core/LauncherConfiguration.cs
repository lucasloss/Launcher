using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace Launcher.Core
{
    /// <summary>
    /// Represents a list of <see cref="ModuleInfo"/> objects and its configurations.
    /// </summary>
    public class LauncherConfiguration
    {
        /// <summary>
        /// Gets a list of <see cref="ModuleInfo"/>.
        /// </summary>
        public BindingList<ModuleInfo> Modules { get; private set; } = new();

        /// <summary>
        /// Loads a new <see cref="LauncherConfiguration"/> from disk.
        /// </summary>
        /// <param name="fileName">The project file to be loaded.</param>
        /// <returns>A new instance of <see cref="LauncherConfiguration"/>.</returns>
        public static LauncherConfiguration? LoadFromFile(string fileName)
        {
            string jsonText = File.ReadAllText(fileName, Encoding.UTF8);
            LauncherConfiguration? launcherConfiguration = JsonConvert.DeserializeObject<LauncherConfiguration>(jsonText);

            if (launcherConfiguration is not null)
            {
                launcherConfiguration.ResetIndexes();
            }

            return launcherConfiguration;
        }

        /// <summary>
        /// Moves the <see cref="ModuleInfo"/> down in the list and updates the indexes of the modules in the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be moved down the list.</param>
        /// <returns>The new index of the <see cref="ModuleInfo"/> in the list.</returns>
        public int MoveDown(ModuleInfo moduleInfo)
        {
            int lastIndexOf = Modules.IndexOf(moduleInfo);

            if (lastIndexOf < Modules.Count - 1)
            {
                int newIndex = lastIndexOf + 1;
                Modules.Remove(moduleInfo);
                Modules.Insert(newIndex, moduleInfo);
                ResetIndexes();
            }

            return Modules.IndexOf(moduleInfo);
        }

        /// <summary>
        /// Moves the <see cref="ModuleInfo"/> up in the list and updates the indexes of the modules in the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be moved up the list.</param>
        /// <returns>The new index of the <see cref="ModuleInfo"/> in the list.</returns>
        public int MoveUp(ModuleInfo moduleInfo)
        {
            int lastIndexOf = Modules.IndexOf(moduleInfo);

            if (lastIndexOf > 0)
            {
                int newIndex = lastIndexOf - 1;
                Modules.Remove(moduleInfo);
                Modules.Insert(newIndex, moduleInfo);
                ResetIndexes();
            }

            return Modules.IndexOf(moduleInfo);
        }

        /// <summary>
        /// Saves the <see cref="LauncherConfiguration"/> to disk.
        /// </summary>
        /// <param name="fileName">The location where the file should be saved, including the name of the file and extension.</param>
        public void SaveToDisk(string fileName)
        {
            JsonSerializerSettings jsonSerializerSettings = new()
            {
                Formatting = Formatting.Indented,
            };

            string contents = JsonConvert.SerializeObject(this, jsonSerializerSettings);
            File.WriteAllText(fileName, contents, Encoding.UTF8);
        }

        /// <summary>
        /// Removes a <see cref="ModuleInfo"/> from the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be removed.</param>
        public void Remove(ModuleInfo moduleInfo)
        {
            Modules.Remove(moduleInfo);
        }

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the list.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to be added.</param>
        public void Add(ModuleInfo moduleInfo)
        {
            Modules.Add(moduleInfo);
        }

        private void ResetIndexes()
        {
            int index = 1;

            foreach (ModuleInfo module in Modules)
            {
                module.Index = index++;
            }
        }
    }
}
