using System.Diagnostics;

namespace Launcher.Core
{
    /// <summary>
    /// A class that contains a <see cref="System.Diagnostics.Process"/> and its file name.
    /// The purpose of this class is to provide the process file name without
    /// the need to access the <see cref="Process.MainModule"/>, which
    /// is very slow and causes serious performance issues.
    /// Note: it is your responsability to copy the file name of the process from the
    /// <see cref="Process.MainModule"/> to the <see cref="FileName"/> property.
    /// </summary>
    public class ProcessContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessContainer"/> class.
        /// </summary>
        /// <param name="process">The <see cref="System.Diagnostics.Process"/>.</param>
        /// <param name="fileName">The file name retrieved from <see cref="Process.MainModule"/>.</param>
        public ProcessContainer(Process process, string fileName)
        {
            if (process is null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or whitespace.", nameof(fileName));
            }

            Process = process;
            FileName = fileName;
        }

        /// <summary>
        /// Gets the <see cref="System.Diagnostics.Process"/>.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Gets the file name (including path) of the <see cref="System.Diagnostics.Process"/>.
        /// Note: it is your responsability to copy the file name of the process to this property.
        /// </summary>
        public string FileName { get; }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return Process.ProcessName;
        }
    }
}
