using System;
using System.ComponentModel;

namespace Habanero.UI.Base
{
    public interface IDataGridViewCell
    {
        /// <summary>Gets the column index for this cell. </summary>
        /// <returns>The index of the column that contains the cell; -1 if the cell is not contained within a column.</returns>
        /// <filterpriority>1</filterpriority>
        int ColumnIndex { get; }

        /// <summary>Gets a value that indicates whether the cell is currently displayed on-screen. </summary>
        /// <returns>true if the cell is on-screen or partially on-screen; otherwise, false.</returns>
        [Browsable(false)]
        bool Displayed { get; }

        /// <summary>Gets a value indicating whether the cell is frozen. </summary>
        /// <returns>true if the cell is frozen; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        bool Frozen { get; }

                /// <summary>Gets the value of the cell as formatted for display.</summary>
        /// <returns>The formatted value of the cell or null if the cell does not belong to a <see cref="IDataGridView"></see> control.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The row containing the cell is a shared row.-or-The cell is a column header cell.</exception>
        /// <exception cref="T:System.Exception">Formatting failed and either there is no handler for the <see cref="IDataGridView.DataError"></see> event of the <see cref="T:Gizmox.WebGUI.Forms.DataGridView"></see> control or the handler set the <see cref="P:Gizmox.WebGUI.Forms.DataGridViewDataErrorEventArgs.ThrowException"></see> property to true. The exception object can typically be cast to type <see cref="T:System.FormatException"></see>.</exception>
        /// <exception cref="T:System.InvalidOperationException"><see cref="IDataGridViewCell.ColumnIndex"></see> is less than 0, indicating that the cell is a row header cell.</exception>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        object FormattedValue { get; }

        /// <summary>Gets a value indicating whether this cell is currently being edited.</summary>
        /// <returns>true if the cell is in edit mode; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">The row containing the cell is a shared row.</exception>
        [Browsable(false)]
        bool IsInEditMode { get; }

        /// <summary>Gets or sets a value indicating whether the cell's data can be edited. </summary>
        /// <returns>true if the cell's data can be edited; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">There is no owning row when setting this property. -or-The owning row is shared when setting this property.</exception>
        /// <filterpriority>1</filterpriority>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool ReadOnly { get; set; }

        /// <summary>Gets the index of the cell's parent row. </summary>
        /// <returns>The index of the row that contains the cell; -1 if there is no owning row.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        int RowIndex { get; }

        /// <summary>Gets or sets a value indicating whether the cell has been selected. </summary>
        /// <returns>true if the cell has been selected; otherwise, false.</returns>
        /// <exception cref="T:System.InvalidOperationException">There is no associated <see cref="IDataGridView"></see> when setting this property. -or-The owning row is shared when setting this property.</exception>
        /// <filterpriority>1</filterpriority>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        bool Selected { get; set; }

        /// <summary>Gets or sets the value associated with this cell. </summary>
        /// <returns>Gets or sets the data to be displayed by the cell. The default is null.</returns>
        /// <exception cref="T:System.InvalidOperationException"><see cref="IDataGridViewCell.ColumnIndex"></see> is less than 0, indicating that the cell is a row header cell.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><see cref="IDataGridViewCell.RowIndex"></see> is outside the valid range of 0 to the number of rows in the control minus 1.</exception>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        object Value { get; set; }

        /// <summary>Gets or sets the data type of the values in the cell. </summary>
        /// <returns>A <see cref="T:System.Type"></see> representing the data type of the value in the cell.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        Type ValueType { get; set; }

        /// <summary>Gets a value indicating whether the cell is in a row or column that has been hidden. </summary>
        /// <returns>true if the cell is visible; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        [Browsable(false)]
        bool Visible { get; }
    }
}