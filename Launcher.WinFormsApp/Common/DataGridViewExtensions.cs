using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    /// <see cref="DataGridView"/> extension methods.
    /// </summary>
    public static class DataGridViewExtensions
    {
        /// <summary>
        /// Extension method used to set the DoubleBuffered property for in a <see cref="DataGridView"/>.
        /// </summary>
        /// <param name="dataGridView">The <see cref="DataGridView"/>.</param>
        /// <param name="value">The value to be assigned to the DoubleBuffered property of the <see cref="DataGridView"/>.</param>
        public static void DoubleBuffered(this DataGridView dataGridView, bool value)
        {
            Type type = dataGridView.GetType();
            PropertyInfo? propertyInfo = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            propertyInfo?.SetValue(dataGridView, value, null);
        }

        /// <summary>
        /// Set the visibility of all columns of a DataGridView.
        /// </summary>
        /// <param name="dataGridView">The <see cref="DataGridView"/>.</param>
        /// <param name="value">A <see cref="bool"/> value that indicates whether the columns should be visible.</param>
        public static void SetColumnsVisibility(this DataGridView dataGridView, bool value)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.Visible = value;
            }
        }

        /// <summary>
        /// Selects a row in the DataGridView.
        /// </summary>
        /// <param name="dataGridView">The <see cref="DataGridView"/>.</param>
        /// <param name="rowIndex">The index of the row to be selected.</param>
        public static void SelectRow(this DataGridView dataGridView, int rowIndex)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }

            if (dataGridView.Rows.Count == 0)
            {
                return;
            }

            if (rowIndex < 0 || rowIndex > dataGridView.Rows.Count - 1)
            {
                return;
            }

            int visibleColumnIndex = dataGridView.GetFirstVisibleColumnIndex();

            if (visibleColumnIndex != -1)
            {
                dataGridView.Rows[rowIndex].Selected = true;
                dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[visibleColumnIndex];
            }
        }

        /// <summary>
        /// Gets the index of the first visible column.
        /// </summary>
        /// <param name="dataGridView">The <see cref="DataGridView"/>.</param>
        /// <returns>
        /// The index of the first visible column. Returns -1 if the <see cref="DataGridView"/> has no columns or no visible columns.
        /// </returns>
        public static int GetFirstVisibleColumnIndex(this DataGridView dataGridView)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }

            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (dataGridView.Columns[i].Visible)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Configures the <see cref="DataGridView"/>'s properties with the default values.
        /// This extesion method provides a way to keep all grids throughout the application with the same configuration.
        /// </summary>
        /// <param name="dataGridView">The <see cref="DataGridView"/>.</param>
        public static void Configure(this DataGridView dataGridView)
        {
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.ReadOnly = true;
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.RowHeadersWidth = 25;
            dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView.ShowCellToolTips = false;
            dataGridView.StandardTab = true;
            dataGridView.DoubleBuffered(true);
        }
    }
}