using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    /// <see cref="Control"/> extension methods.
    /// </summary>
    public static class ControlExtensions
    {
        // Source: https://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Keep the original name of the message id.")]
        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// Sets a value indicating whether this control should redraw its surface using a secondary buffer to reduce or prevent flicker.
        /// </summary>
        /// <param name="control">The <see cref="Control"/>.</param>
        /// <param name="value">The value that is going to be set to the <see cref="Control.DoubleBuffered"/> property.</param>
        public static void DoubleBuffered(this Control control, bool value)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (SystemInformation.TerminalServerSession)
            {
                return;
            }

            Type type = control.GetType();
            PropertyInfo? propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfo?.SetValue(control, value, null);
        }

        /// <summary>
        /// Suspends the update on a given <see cref="Control"/>.
        /// </summary>
        /// <param name="control">The <see cref="Control"/>.</param>
        public static void SuspendUpdate(this Control control)
        {
            // Source: https://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children.
            Message message = Message.Create(
                control.Handle,
                WM_SETREDRAW,
                IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref message);
        }

        /// <summary>
        /// Resumes the update on a given <see cref="Control"/>.
        /// </summary>
        /// <param name="control">The <see cref="Control"/>.</param>
        public static void ResumeUpdate(this Control control)
        {
            // Source: https://stackoverflow.com/questions/487661/how-do-i-suspend-painting-for-a-control-and-its-children.
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message message = Message.Create(
                control.Handle,
                WM_SETREDRAW,
                wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref message);
            control.Invalidate();
        }
    }
}